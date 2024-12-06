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

    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync(int page, int pageSize) 
    {
        var users = await _userRepository.GetAllUsersAsync(page, pageSize);
        var dtos = users.Select(user => _mapper.Map<UserDTO>(user)).ToList();
        return dtos;
    }

    public async Task<UserDTO?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetUserByIdAsync(new UserId(id));
        return user != null? _mapper.Map<UserDTO>(user) : null;
    }

    public async Task<UserDTO?> UpdateUserAsync(Guid id, UserUpdateDTO dto)
    {
        var user = _mapper.Map<User>(dto);
        var res = await _userRepository.UpdateUserAsync(new UserId(id), user);

        return res != null ? _mapper.Map<UserDTO>(res) : null;
    }

    public async Task<UserDTO?> DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.DeleteUserAsync(new UserId(id));
        var res = _mapper.Map<UserDTO>(user);

        return res ?? null;
    }

    public Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto)
    {
        throw new NotImplementedException();
    }    
}