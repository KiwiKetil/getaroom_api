using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync(int page, int pageSize);
    Task<User?> GetByIdAsync(UserId Id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> UpdateAsync(UserId id, User user);
    Task<User?> DeleteAsync(UserId id);
    Task<User?> RegisterUserAsync(User user);
}
