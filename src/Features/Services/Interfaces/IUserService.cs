using RoomSchedulerAPI.Features.Models.DTOs;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetAllAsync();
    Task<UserDTO?> GetByIdAsync(UserId Id);
    Task<UserDTO?> UpdateAsync(UserId Id, UserUpdateDTO dto);
    Task<UserDTO?> DeleteAsync(UserId Id);
    Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto);
}