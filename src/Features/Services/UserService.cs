using AutoMapper;
using GetARoomAPI.Core.DB.UnitOFWork.Interfaces;
using GetARoomAPI.Features.Models.DTOs.UserDTOs;
using GetARoomAPI.Features.Models.Entities;
using GetARoomAPI.Features.Models.Enums;
using GetARoomAPI.Features.Repositories.Interfaces;
using GetARoomAPI.Features.Services.Interfaces;
using MySqlX.XDevAPI;
using System.Linq;
using System.Security.Claims;

namespace GetARoomAPI.Features.Services;

public class UserService(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository, 
    IPasswordVerificationService passwordVerificationService,
    IPasswordHistoryRepository passwordHistoryRepository,
    ITokenGenerator tokenGenerator, 
    IMapper mapper,
    IUnitOfWorkFactory unitOfWorkFactory, 
    IHttpContextAccessor httpContextAccessor, 
    IRegistrationConfirmationService registrationConfirmationService,
    ILogger<UserService> logger) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly IPasswordVerificationService _passwordVerificationService = passwordVerificationService;
    private readonly IPasswordHistoryRepository _passwordHistoryRepository = passwordHistoryRepository;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory = unitOfWorkFactory;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IRegistrationConfirmationService _registrationConfirmationService = registrationConfirmationService;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<UsersWithCountDTO> GetUsersAsync(UserQuery query)
    {
        _logger.LogDebug("Retrieving users");

        var claimsPrincipal = _httpContextAccessor.HttpContext?.User!;
        var isAdmin = claimsPrincipal.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value.Equals("admin", StringComparison.OrdinalIgnoreCase));

        if (!isAdmin)
        {
            query = query with { Roles = ["Client"] };
        }

        var (users, totalCount) = await _userRepository.GetUsersAsync(query, isAdmin);
        var dtos = users.Select(user => _mapper.Map<UserDTO>(user)).ToList();
        return new UsersWithCountDTO(totalCount, dtos);
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid id)
    {
        _logger.LogDebug("Retrieving user with ID {userId}", id);

        var user = await _userRepository.GetUserByIdAsync(new UserId(id));

        var userDTO = _mapper.Map<UserDTO>(user);
        return userDTO;
    }

    public async Task<UserDTO?> UpdateUserAsync(Guid id, UserUpdateDTO dto)
    {
        _logger.LogDebug("Updating user with ID {userId}", id);  

        var user = _mapper.Map<User>(dto);
        var res = await _userRepository.UpdateUserAsync(new UserId(id), user);

        var userDTO = _mapper.Map<UserDTO>(res);
        return userDTO;
    }

    public async Task<UserDTO?> DeleteUserAsync(Guid id)
    {
        _logger.LogDebug("Deleting user with ID {userId}", id);

        var userTodeleteRoles = await _userRoleRepository.GetUserRolesAsync(new UserId(id));
        if (userTodeleteRoles.Any(role => role.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
        {
            throw new UnauthorizedAccessException("Admin users cannot be deleted.");
        }

        var user = await _userRepository.DeleteUserAsync(new UserId(id));

        var userDTO = _mapper.Map<UserDTO>(user);
        return userDTO;
    }

    public async Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto, UserRoles role)
    {
        _logger.LogInformation("Registering user");

        var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            _logger.LogDebug("user already exists");
            return null;
        }

        var user = _mapper.Map<User>(dto);
        user.Id = UserId.NewId;
        user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var unitOfWork = _unitOfWorkFactory.Create();
        try
        {
            await unitOfWork.BeginAsync();

            var res = await _userRepository.RegisterUserAsync(user, unitOfWork);

            bool roleAdded = role switch
            {
                UserRoles.Employee => await _userRoleRepository.AddUserRoleAsync(user.Id, UserRoles.Employee, unitOfWork),
                UserRoles.Client => await _userRoleRepository.AddUserRoleAsync(user.Id, UserRoles.Client, unitOfWork),
                _ => throw new ArgumentException("Invalid role", nameof(role))
            };

            if (!roleAdded)
            {
                throw new InvalidOperationException("Failed to assign role to the user.");
            }

            await unitOfWork.CommitAsync();
            _logger.LogInformation("Committed UoW with ID: {UnitOfWorkId}", unitOfWork.Id);
           
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error in UoW with ID: {UnitOfWorkId}. Rolling back...", unitOfWork.Id);
            await unitOfWork.RollbackAsync();
            return null;
        }
        try
        {
            await _registrationConfirmationService.SendConfirmationEmailAsync(user.Id, user.Email);
        }
        catch (Exception emailEx)
        {
            _logger.LogError(emailEx, "Error sending confirmation email. The user was registered but email confirmation failed.");
        }
        var userDTO = _mapper.Map<UserDTO>(user);
        return userDTO;
    }

    public async Task<string?> UserLoginAsync(LoginDTO dto)
    {
        _logger.LogDebug("User logging in");

        var user = await _userRepository.GetUserByEmailAsync(dto.Username);
        if (user == null)
        {
            _logger.LogDebug("User not found");
            return null;
        }

        var verifiedUser = _passwordVerificationService.VerifyPassword(dto, user);
        if (!verifiedUser)
        {
            _logger.LogDebug("Could not verify password");
            return null;
        }
        var hasConfirmedRegistrationTask = _registrationConfirmationService.HasConfirmedRegistrationAsync(user.Id);

        var userRolesTask = _userRoleRepository.GetUserRolesAsync(user.Id);
        await Task.WhenAll(hasConfirmedRegistrationTask, userRolesTask);
        var hasConfirmedRegistration = await hasConfirmedRegistrationTask;
        var userRoles = await userRolesTask;

        var token = _tokenGenerator.GenerateToken(user, hasConfirmedRegistration, userRoles);
        return token;
    }

    public async Task<string?> UpdatePasswordAsync(UpdatePasswordDTO dto)
    {
        _logger.LogDebug("Updating password");

        var user = await _userRepository.GetUserByEmailAsync(dto.Username);
        if (user is null)
        {
            _logger.LogInformation("User not found");
            return null;
        }

        var verified = _passwordVerificationService.VerifyPassword(dto, user);
        if (!verified)
        {
            _logger.LogInformation("Verification failed");
            return null;
        }

        string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        var unitOfWork = _unitOfWorkFactory.Create();
        try
        {
            await unitOfWork.BeginAsync();

            var updateSuccess = await _userRepository.UpdatePasswordAsync(user.Id, newHashedPassword, unitOfWork);
            if (!updateSuccess)
            {
                throw new InvalidOperationException("Failed to update user password.");
            }

            var historySuccess = await _passwordHistoryRepository.InsertPasswordUpdateRecordAsync(user.Id.Value, unitOfWork);
            if (!historySuccess)
            {
                throw new InvalidOperationException("Password history record was not inserted.");
            }

            await unitOfWork.CommitAsync();
            _logger.LogInformation("Committed UoW with ID: {UnitOfWorkId}", unitOfWork.Id);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error in UoW with ID: {UnitOfWorkId}. Rolling back...", unitOfWork.Id);
            await unitOfWork.RollbackAsync();
            return null;
        }

        _logger.LogInformation("Password updated successfully for user {Email}", dto.Username);

        var userRoles = await _userRoleRepository.GetUserRolesAsync(user.Id);
        var token = _tokenGenerator.GenerateToken(user, true, userRoles);
        return token;
    } 
}
