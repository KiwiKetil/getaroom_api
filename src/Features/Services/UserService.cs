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

    public async Task<IEnumerable<UserDTO>> GetAllAsync(int page, int pageSize) 
    {
        _logger.LogInformation("Retrieving all users");

        var users = await _userRepository.GetAllAsync(page, pageSize);
        var dtos = users.Select(user => _mapper.Map<UserDTO>(user)).ToList();
        return dtos;
    }

    public async Task<UserDTO?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Retrieving user with ID {user}", id);

        var user = await _userRepository.GetByIdAsync(new UserId(id));
        return user != null? _mapper.Map<UserDTO>(user) : null;
    }

    public async Task<UserDTO?> UpdateAsync(Guid id, UserUpdateDTO dto)
    {
        _logger.LogInformation("Updating user with ID {userId}", id);

        var userId = new UserId(id);

        var userExist = _userRepository.GetByIdAsync(userId);
        if (userExist == null) 
        {
            _logger.LogInformation("User does not exist {user}", id);
        }

        var user = _mapper.Map<User>(dto);

        var res = await _userRepository.UpdateAsync(userId, user);
        var updatedUser = _mapper.Map<UserDTO>(res);

        return updatedUser;
    }

    public Task<UserDTO?> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto)
    {
        throw new NotImplementedException();
    }    
}