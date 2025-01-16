using AutoMapper;
using RoomSchedulerAPI.Features.Models.DTOs;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.Features.Services;

public class UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<UserService> _logger = logger;
    private readonly IMapper _mapper = mapper;

    public async Task<(IEnumerable<UserDTO> Users, int TotalCount)> GetUsersAsync(UserQuery query)
    {
        _logger.LogDebug("Retrieving all users");

        var (users, totalCount) = await _userRepository.GetUsersAsync(query);
        var dtos = users.Select(user => _mapper.Map<UserDTO>(user)).ToList();
        return (dtos,totalCount);
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid id)
    {
        _logger.LogDebug("Retrieving user with ID {userId}", id);

        var user = await _userRepository.GetUserByIdAsync(new UserId(id));
        return user != null ? _mapper.Map<UserDTO>(user) : null;
    }

    public async Task<UserDTO?> UpdateUserAsync(Guid id, UserUpdateDTO dto)
    {
        _logger.LogDebug("Updating user with ID {userId}", id);

        var user = _mapper.Map<User>(dto);
        var res = await _userRepository.UpdateUserAsync(new UserId(id), user);

        return res != null ? _mapper.Map<UserDTO>(res) : null;
    }

    public async Task<UserDTO?> DeleteUserAsync(Guid id)
    {
        _logger.LogDebug("Deleting user with ID {userId}", id);

        var user = await _userRepository.DeleteUserAsync(new UserId(id));
        var res = _mapper.Map<UserDTO>(user);

        return res ?? null;
    }

    public async Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            _logger.LogDebug("User already exists");
            return null;
        }

        var user = _mapper.Map<User>(dto);
        user.Id = UserId.NewId;
        user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        user.Salt = BCrypt.Net.BCrypt.GenerateSalt();

        var res = await _userRepository.RegisterUserAsync(user);
        var userDTO = _mapper.Map<UserDTO>(res);
        return userDTO;
    }
}