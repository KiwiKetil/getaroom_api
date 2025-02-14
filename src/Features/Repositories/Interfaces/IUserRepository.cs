using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Repositories.Interfaces;

public interface IUserRepository
{
    Task<(IEnumerable<User> Users, int TotalCount)> GetUsersAsync(UserQuery query);
    Task<User?> GetUserByIdAsync(UserId Id);
    Task<User?> UpdateUserAsync(UserId id, User user);
    Task<User?> DeleteUserAsync(UserId id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> RegisterUserAsync(User user);
    Task<bool> UpdatePasswordAsync(UserId id, string newHashedPassword);
}
