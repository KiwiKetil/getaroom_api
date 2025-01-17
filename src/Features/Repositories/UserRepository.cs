using Dapper;
using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using RoomSchedulerAPI.Features.Models.DTOs;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;

namespace RoomSchedulerAPI.Features.Repositories;

public class UserRepository(IDbConnectionFactory mySqlConnectionFactory, ILogger<UserRepository> logger) : IUserRepository
{
    private readonly IDbConnectionFactory _mySqlConnectionFactory = mySqlConnectionFactory;
    private readonly ILogger<UserRepository> _logger = logger;

    public async Task<(IEnumerable<User> Users, int TotalCount)> GetUsersAsync(UserQuery query)
    {
        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();

        var baseSql = "FROM Users WHERE 1=1";
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.FirstName))
        {
            baseSql += " AND FirstName LIKE @FirstName";
            parameters.Add("FirstName", $"{query.FirstName}%");
        }

        if (!string.IsNullOrEmpty(query.LastName))
        {
            baseSql += " AND LastName LIKE @LastName";
            parameters.Add("LastName", $"{query.LastName}%");
        }

        if (!string.IsNullOrEmpty(query.PhoneNumber))
        {
            baseSql += " AND PhoneNumber = @PhoneNumber";
            parameters.Add("PhoneNumber", query.PhoneNumber);
        }

        if (!string.IsNullOrEmpty(query.Email))
        {
            baseSql += " AND Email LIKE @Email";
            parameters.Add("Email", $"{ query.Email}%");
        }

        // Query to get total count
        var countSql = $"SELECT COUNT(*) {baseSql}";
        var totalCount = await dbConnection.ExecuteScalarAsync<int>(countSql, parameters);
                
        var skipNumber = (query.Page - 1) * query.PageSize;

        // Handle sorting and pagination
        var dataSql = $"SELECT * {baseSql} ORDER BY {query.SortBy} {query.Order} LIMIT @PageSize OFFSET @SkipNumber";
        parameters.Add("PageSize", query.PageSize);
        parameters.Add("SkipNumber", skipNumber);

        parameters.Add("PageSize", query.PageSize);
        parameters.Add("SkipNumber", skipNumber);

        var users = await dbConnection.QueryAsync<User>(dataSql, parameters);

        // Return both users and total count
        return (users, totalCount);
    }

    public async Task<User?> GetUserByIdAsync(UserId id)
    {
        _logger.LogDebug("Retrieving user with ID {userId} from DB", id);

        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();

        string getUserSql = @"SELECT Id, FirstName, LastName, PhoneNumber, Email FROM Users where Id = @id";
        return await dbConnection.QueryFirstOrDefaultAsync<User>(getUserSql, new { id = id.Value });
    }

    public async Task<User?> UpdateUserAsync(UserId id, User user)
    {
        _logger.LogDebug("Updating user with ID {userId} in DB", id);

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
        _logger.LogInformation("Deleting user with ID {userId} in DB", id);

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
            _logger.LogError("ERROR: Delete attempted for user with ID {userId} resulted in {rowsAffected} rows affected. Transaction rolled back to " +
                "maintain data integrity.", id, rowsAffected);
            return null;
        }

        transaction.Commit();
        _logger.LogInformation("User with ID {userId} successfully deleted.", id);
        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        _logger.LogInformation("Retrieving user with email {email} in DB", email);

        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();

        var emailSql = @"SELECT Id, FirstName, LastName, PhoneNumber, Email FROM Users WHERE email = @email";

        return await dbConnection.QueryFirstOrDefaultAsync<User>(emailSql, new { email });
    }

    public async Task<User?> RegisterUserAsync(User user)
    {
        _logger.LogDebug("Adding new user to DB");

        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();
        using var transaction = dbConnection.BeginTransaction();

        var registerUserSql = @"INSERT into USERS(Id, FirstName, LastName, PhoneNumber, Email, HashedPassword, Salt)
                                VALUES (@Id, @FirstName, @LastName, @PhoneNumber, @Email, @HashedPassword, @Salt)";

        var parameters = new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.Email,
            user.HashedPassword,
            user.Salt
        };

        var userTableRowsAffected = await dbConnection.ExecuteAsync(registerUserSql, parameters, transaction);

        if (userTableRowsAffected > 1)
        {
            transaction.Rollback();
            _logger.LogError("ERROR: Registering user resulted in {rowsAffected} rows affected. Transaction rolled back to " +
                "maintain data integrity.", userTableRowsAffected);
            return null;
        }       
        
        transaction.Commit();

        string getNewUserSql = @"SELECT Id, FirstName, LastName, PhoneNumber, Email FROM Users WHERE Email = @Email";
        var registeredUser = await dbConnection.QueryFirstOrDefaultAsync<User>(getNewUserSql, new { Email = user.Email }, transaction);

        _logger.LogInformation("New user successfully registered.");
        return registeredUser;
    }
}
