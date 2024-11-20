using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync(int page, int pageSize);
    Task<User?> GetUserByIdAsync(UserId Id);
    Task<User?> UpdateUserAsync(UserId id, User user);
    Task<User?> DeleteUserAsync(UserId id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> RegisterUserAsync(User user);
}
