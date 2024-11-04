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

    public async Task<IEnumerable<UserDTO>> GetAllAsync() // paginering
    {
        _logger.LogInformation("Retrieving all users");
        var users = await _userRepository.GetAllAsync();
        var dtos = users.Select(user => _mapper.Map<UserDTO>(user)).ToList();
        return dtos;
    }

    public Task<UserDTO?> GetByIdAsync(UserId Id)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO?> UpdateAsync(UserId Id, UserUpdateDTO dto)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO?> DeleteAsync(UserId Id)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto)
    {
        throw new NotImplementedException();
    }    
}