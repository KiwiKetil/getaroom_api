using Dapper;
using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using System.Data;

namespace RoomSchedulerAPI.Features.Repositories;

public class UserRepository(IDbConnectionFactory mySqlConnectionFactory, ILogger<UserRepository> logger) : IUserRepository
{
    private readonly IDbConnectionFactory _mySqlConnectionFactory = mySqlConnectionFactory;
    private readonly ILogger<UserRepository> _logger = logger;

    public async Task<IEnumerable<User>> GetAllAsync(int page, int pageSize)
    {
        _logger.LogInformation("Retrieving all users from db");

        using IDbConnection dbConnection = _mySqlConnectionFactory.CreateConnection();

        var skipNumber = (page - 1) * pageSize;

        string sql = "SELECT Id, FirstName, LastName, PhoneNumber, Email FROM Users LIMIT @pageSize OFFSET @skipNumber"; 
        var users = await dbConnection.QueryAsync<User>(sql, new { pageSize, skipNumber });

        return users;
    }

    public async Task<User?> GetByIdAsync(UserId id)
    {
        _logger.LogInformation("Retrieving user from db");

        using IDbConnection dbConnection = _mySqlConnectionFactory.CreateConnection();

        string sql = "SELECT * FROM Users where Id = @id";
        return await dbConnection.QueryFirstOrDefaultAsync<User>(sql, new { id = id.Value });
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> UpdateAsync(UserId id, User user)
    {
        throw new NotImplementedException();
    }

    public Task<User?> DeleteAsync(UserId id)
    {
        throw new NotImplementedException();
    }

    public Task<User?> RegisterUserAsync(User user)
    {
        throw new NotImplementedException();
    }
}