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

    public async Task<IEnumerable<User>> GetAllUsersAsync(int page, int pageSize)
    {
        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync(); 

        var skipNumber = (page - 1) * pageSize;

        string getUsersSql = @"SELECT Id, FirstName, LastName, PhoneNumber, Email
                FROM Users 
                ORDER BY LastName
                LIMIT @pageSize OFFSET @skipNumber"; 
        var users = await dbConnection.QueryAsync<User>(getUsersSql, new { pageSize, skipNumber });

        return users;
    }

    public async Task<User?> GetUserByIdAsync(UserId id)
    {
        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync(); 

        string getUserSql = @"SELECT Id, FirstName, LastName, PhoneNumber, Email FROM Users where Id = @id";
        return await dbConnection.QueryFirstOrDefaultAsync<User>(getUserSql, new { id = id.Value });
    }

    public async Task<User?> UpdateUserAsync(UserId id, User user)
    {
        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();
        using var transaction = dbConnection.BeginTransaction();

        string getUserSql = @"SELECT Id, FirstName, LastName, PhoneNumber, Email FROM Users WHERE Id = @Id";
        var foundUser = await dbConnection.QueryFirstOrDefaultAsync<User>(getUserSql, new { Id = id.Value }, transaction);

        if (foundUser == null)
        {
            _logger.LogWarning("Update failed for user with ID {userId}. User was not found.", id);
            return null;
        }

        string updateUserSql = @"UPDATE Users
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

        int rowsAffected = await dbConnection.ExecuteAsync(updateUserSql, parameters, transaction); 

        if (rowsAffected > 1)
        {
            transaction.Rollback(); 
            _logger.LogError("Update attempted for user with ID {userId} resulted in multiple rows affected. Transaction rolled back to maintain data integrity.", id);
            return null;
        }

        transaction.Commit();
        _logger.LogInformation("User with ID {userId} successfully updated.", id);
        return await dbConnection.QueryFirstOrDefaultAsync<User>(getUserSql, new { Id = id.Value });           
    }

    public async Task<User?> DeleteUserAsync(UserId id)
    {
        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();
        using var transaction = dbConnection.BeginTransaction();

        string getUserSql = @"SELECT Id, FirstName, LastName, PhoneNumber, Email FROM Users WHERE Id = @Id";
        var user = await dbConnection.QueryFirstOrDefaultAsync<User>(getUserSql, new { Id = id.Value }, transaction);

        if (user == null) 
        {
            _logger.LogWarning("Delete failed for user with ID {userId}. User was not found.", id);
            return null;
        }

        var deleteUserSql = "DELETE FROM Users WHERE Id = @Id";
        int rowsAffected = await dbConnection.ExecuteAsync(deleteUserSql, new { Id = id.Value }, transaction);

        if (rowsAffected > 1)
        {
            transaction.Rollback();
            _logger.LogError("ERROR: Delete attempted for user with ID {userId} resulted in {rowsAffected} rows affected. Transaction rolled back to maintain data integrity.", id, rowsAffected);
            return null;       
        }

        transaction.Commit();
        _logger.LogInformation("User with ID {userId} successfully deleted.", id);
        return user;
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> RegisterUserAsync(User user)
    {
        throw new NotImplementedException();
    } 
}