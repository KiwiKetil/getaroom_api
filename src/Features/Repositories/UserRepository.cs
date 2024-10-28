using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;

namespace RoomSchedulerAPI.Features.Repositories;

public class UserRepository : IUserRepository
{
    public Task<IEnumerable<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(UserId Id)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> UpdateAsync(UserId Id, User user)
    {
        throw new NotImplementedException();
    }

    public Task<User?> DeleteAsync(UserId Id)
    {
        throw new NotImplementedException();
    }

    public Task<User?> RegisterUserAsync(User user)
    {
        throw new NotImplementedException();
    }
}