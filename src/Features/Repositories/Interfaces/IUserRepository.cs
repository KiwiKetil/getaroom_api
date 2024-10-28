using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(UserId Id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> UpdateAsync(UserId Id, User user);
    Task<User?> DeleteAsync(UserId Id);
    Task<User?> RegisterUserAsync(User user);
}
