using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface IUserService
{
    Task<UsersAndCountDTO> GetUsersAsync(UserQuery query); 
    Task<UserDTO?> GetUserByIdAsync(Guid id);
    Task<UserDTO?> UpdateUserAsync(Guid id, UserUpdateDTO dto);
    Task<UserDTO?> DeleteUserAsync(Guid id);
    Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto);
    Task<bool> UpdatePasswordAsync(UpdatePasswordDTO dto, User user); 
    Task<bool> HasUpdatedPassword(UserId id);
    Task<User?> GetUserByEmailAsync(string email);
}