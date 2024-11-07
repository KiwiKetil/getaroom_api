using Dapper;
using Mysqlx.Crud;
using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using System.Data;
using System.Data.Common;

namespace RoomSchedulerAPI.Features.Repositories;

public class UserRepository(IDbConnectionFactory mySqlConnectionFactory, ILogger<UserRepository> logger) : IUserRepository
{
    private readonly IDbConnectionFactory _mySqlConnectionFactory = mySqlConnectionFactory;
    private readonly ILogger<UserRepository> _logger = logger;

    public async Task<IEnumerable<User>> GetAllAsync(int page, int pageSize)
    {
        _logger.LogInformation("Retrieving all users from DB");

        using IDbConnection dbConnection = _mySqlConnectionFactory.CreateConnection();

        var skipNumber = (page - 1) * pageSize;

        string sql = @"SELECT Id, FirstName, LastName, PhoneNumber, Email
                FROM Users 
                ORDER BY LastName
                LIMIT @pageSize OFFSET @skipNumber"; 
        var users = await dbConnection.QueryAsync<User>(sql, new { pageSize, skipNumber });

        return users;
    }

    public async Task<User?> GetByIdAsync(UserId id)
    {
        _logger.LogInformation("Retrieving user with ID {userId} from DB", id);

        using IDbConnection dbConnection = _mySqlConnectionFactory.CreateConnection();

        string sql = "SELECT * FROM Users where Id = @id";
        return await dbConnection.QueryFirstOrDefaultAsync<User>(sql, new { id = id.Value });
    }  

    public async Task<User?> UpdateAsync(UserId id, User user)
    {
        _logger.LogInformation("Updating user with ID {userId} in DB", id);

        using IDbConnection dbConnection = _mySqlConnectionFactory.CreateConnection();

        string sql = @"UPDATE Users
                    SET
                        FirstName = @FirstName,
                        LastName = @LastName,
                        PhoneNumber = @PhoneNumber,
                        Email = @Email
                    WHERE
                        Id = @Id";

        var parameters = new
        {           
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.Email,
            Id = id.Value
        };

        int rowsAffected = await dbConnection.ExecuteAsync(sql, parameters);

        if (rowsAffected > 0) 
        {
            string selectSql = "SELECT * FROM Users WHERE Id = @Id";
            return await dbConnection.QueryFirstOrDefaultAsync<User>(selectSql, new { Id = id.Value });
        }
        return null;
    }

    public Task<User?> DeleteAsync(UserId id)
    {
        throw new NotImplementedException();
    }
    
    public Task<User?> RegisterUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }
}