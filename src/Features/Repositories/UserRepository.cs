using Dapper;
using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using System.Data;

namespace RoomSchedulerAPI.Features.Repositories;

public class UserRepository(IDbConnectionFactory mySqlConnectionFactory) : IUserRepository
{
    private readonly IDbConnectionFactory _mySqlConnectionFactory = mySqlConnectionFactory;

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using IDbConnection dbConnection = _mySqlConnectionFactory.CreateConnection();

        string sql = "SELECT Id, FirstName, LastName, PhoneNumber, Email FROM Users"; 
        var users = await dbConnection.QueryAsync<User>(sql);

        return users;
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