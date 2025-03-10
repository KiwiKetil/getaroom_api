using AutoMapper;
using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using RoomSchedulerAPI.Core.DB.UnitOFWork;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.Features.Services;

public class UserService(IUserRepository userRepository, IUserRoleRepository userRoleRepository, IPasswordVerificationService passwordVerificationService,
    IPasswordHistoryRepository passwordHistoryRepository, ITokenGenerator tokenGenerator, IMapper mapper, IDbConnectionFactory mySqlConnectionFactory, ILogger<UserService> logger) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly IPasswordVerificationService _passwordVerificationService = passwordVerificationService;
    private readonly IPasswordHistoryRepository _passwordHistoryRepository = passwordHistoryRepository;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IMapper _mapper = mapper;
    private readonly IDbConnectionFactory _mySqlConnectionFactory = mySqlConnectionFactory;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<UsersWithCountDTO> GetUsersAsync(UserQuery query)
    {
        _logger.LogDebug("Retrieving users");

        var (users, totalCount) = await _userRepository.GetUsersAsync(query);
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

        var user = await _userRepository.DeleteUserAsync(new UserId(id));

        var userDTO = _mapper.Map<UserDTO>(user);
        return userDTO;
    }

    public async Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto)
    {
        _logger.LogDebug("Registering user");

        var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            _logger.LogDebug("User already exists");
            return null;
        }

        var user = _mapper.Map<User>(dto);
        user.Id = UserId.NewId;
        user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var res = await _userRepository.RegisterUserAsync(user);

        var userDTO = _mapper.Map<UserDTO>(res);
        return userDTO;
    }

    public async Task<string?> UserLoginAsync(LoginDTO dto)
    {
        _logger.LogDebug("User logging in");

        var user = await _userRepository.GetUserByEmailAsync(dto.Email);
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
        var hasUpdatedPasswordTask = _passwordHistoryRepository.PasswordUpdateExistsAsync(user.Id);
        var userRolesTask = _userRoleRepository.GetUserRolesAsync(user.Id);
        await Task.WhenAll(hasUpdatedPasswordTask, userRolesTask);
        var hasUpdatedPassword = await hasUpdatedPasswordTask;
        var userRoles = await userRolesTask;

        var token = _tokenGenerator.GenerateToken(user, hasUpdatedPassword, userRoles);
        return token;
    }

    public async Task<string?> UpdatePasswordAsync(UpdatePasswordDTO dto)
    {
        _logger.LogDebug("Updating password for user {Email}", dto.Email);

        var user = await _userRepository.GetUserByEmailAsync(dto.Email);
        if (user is null)
        {
            _logger.LogError("User not found by email {Email}", dto.Email);
            return null;
        }

        var verified = _passwordVerificationService.VerifyPassword(dto, user);
        if (!verified)
        {
            _logger.LogError("Verification faield");
            return null;
        }

        string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        using var uow = new UnitOFWork(_mySqlConnectionFactory);
        try
        {
            await uow.BeginAsync();

            var updateSuccess = await _userRepository.UpdatePasswordAsync(uow, user.Id, newHashedPassword);
            if (!updateSuccess)
            {
                throw new InvalidOperationException("Failed to update user password.");
            }

            var historySuccess = await _passwordHistoryRepository.InsertPasswordUpdateRecordAsync(uow, user.Id.Value);
            if (!historySuccess)
            {
                throw new InvalidOperationException("Password history record was not inserted.");
            }

            await uow.CommitAsync();
            _logger.LogInformation("Committed UoW with ID: {UnitOfWorkId}", uow.Id);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error in UoW with ID: {UnitOfWorkId}. Rolling back...", uow.Id);
            await uow.RollbackAsync();
            return null;
        }

        _logger.LogInformation("Password updated successfully for user {Email}", dto.Email);

        var userRoles = await _userRoleRepository.GetUserRolesAsync(user.Id);
        var token = _tokenGenerator.GenerateToken(user, true, userRoles);
        return token;
    }
}
