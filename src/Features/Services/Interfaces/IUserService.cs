using RoomSchedulerAPI.Features.Models.DTOs;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetAllAsync(int page, int pageSize);
    Task<UserDTO?> GetByIdAsync(Guid id);
    Task<UserDTO?> UpdateAsync(Guid id, UserUpdateDTO dto);
    Task<UserDTO?> DeleteAsync(Guid id);
    Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto);
}