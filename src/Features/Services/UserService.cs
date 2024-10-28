using RoomSchedulerAPI.Features.Models.DTOs;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.Features.Services;

public class UserService : IUserService
{
    public Task<IEnumerable<UserDTO>> GetAllAsync()
    {
        throw new NotImplementedException();
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