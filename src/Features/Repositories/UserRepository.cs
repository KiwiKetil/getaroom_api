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

        try
        {
            string getUserSql = @"SELECT Id, FirstName, LastName, PhoneNumber, Email FROM Users WHERE Id = @Id";
            var foundUser = await dbConnection.QueryFirstOrDefaultAsync<User>(getUserSql, new { Id = id.Value }, transaction);

            if (foundUser == null)
            {
                _logger.LogWarning("Update failed for user with ID {userId}. User was not found.", id);
                return null;
            }

            string updateUserSql = @"UPDATE Users
                              SET FirstName = @FirstName,
                                  LastName = @LastName,
                                  PhoneNumber = @PhoneNumber,
                                  Email = @Email
                              WHERE Id = @Id";

            var parameters = new
            {
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.Email,
                Id = id.Value
            };

            int rowsAffected = await dbConnection.ExecuteAsync(updateUserSql, parameters, transaction);

            if (rowsAffected != 1)
            {
                throw new InvalidOperationException($"Unexpected rows affected ({rowsAffected}) when updating user with ID {id}");
            }

            var updatedUser = await dbConnection.QueryFirstOrDefaultAsync<User>(getUserSql, new { Id = id.Value }, transaction);

            transaction.Commit();
            _logger.LogInformation("User with ID {userId} successfully updated.", id);

            return updatedUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {userId}. Rolling back transaction.", id);
            transaction.Rollback();
            return null;
        }
    }

    public async Task<User?> DeleteUserAsync(UserId id)
    {
        _logger.LogInformation("Deleting user with ID {userId} in DB", id);

        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();
        using var transaction = dbConnection.BeginTransaction();

        try
        {
            string getUserSql = @"SELECT Id, FirstName, LastName, PhoneNumber, Email FROM Users WHERE Id = @Id";
            var user = await dbConnection.QueryFirstOrDefaultAsync<User>(getUserSql, new { Id = id.Value }, transaction);

            if (user == null)
            {
                _logger.LogWarning("Delete failed for user with ID {userId}. User was not found.", id);
                return null;
            }

            string deleteUserSql = "DELETE FROM Users WHERE Id = @Id";
            int rowsAffected = await dbConnection.ExecuteAsync(deleteUserSql, new { Id = id.Value }, transaction);

            if (rowsAffected != 1)
            {
                throw new InvalidOperationException($"Unexpected rows affected ({rowsAffected}) when deleting user with ID {id}");
            }

            transaction.Commit();
            _logger.LogInformation("User with ID {userId} successfully deleted.", id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {userId}. Rolling back transaction.", id);
            transaction.Rollback();
            return null;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        _logger.LogInformation("Retrieving user with email {email} in DB", email);

        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();

        var getUserbyEmailSql = @"SELECT Id, FirstName, LastName, PhoneNumber, Email, HashedPassword FROM Users WHERE email = @email";

        return await dbConnection.QueryFirstOrDefaultAsync<User>(getUserbyEmailSql, new { email });
    }

    public async Task<User?> RegisterUserAsync(User user)
    {
        _logger.LogDebug("Adding new user to DB");

        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();
        using var transaction = dbConnection.BeginTransaction();

        try
        {
            string registerUserSql = @"INSERT INTO Users (Id, FirstName, LastName, PhoneNumber, Email, HashedPassword)
                                   VALUES (@Id, @FirstName, @LastName, @PhoneNumber, @Email, @HashedPassword)";

            var parameters = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.Email,
                user.HashedPassword
            };

            int rowsAffected = await dbConnection.ExecuteAsync(registerUserSql, parameters, transaction);

            if (rowsAffected != 1)
            {
                throw new InvalidOperationException($"Unexpected rows affected ({rowsAffected}) while registering user.");
            }

            string getNewUserSql = @"SELECT Id, FirstName, LastName, PhoneNumber, Email FROM Users WHERE Id = @Id";
            var registeredUser = await dbConnection.QueryFirstOrDefaultAsync<User>(getNewUserSql, new { user.Id }, transaction);

            transaction.Commit();
            _logger.LogInformation("New user successfully registered.");

            return registeredUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user. Rolling back transaction.");
            transaction.Rollback();
            return null;
        }
    }

    public async Task<bool> ChangePasswordAsync(UserId id, string newHashedPassword)
    {
        _logger.LogDebug("Updating password in DB");

        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();
        using var transaction = dbConnection.BeginTransaction();

        try
        {
            string updateUserPasswordSql = "UPDATE Users SET HashedPassword = @HashedPassword WHERE Id = @Id";
            int rowsAffected = await dbConnection.ExecuteAsync(updateUserPasswordSql,
                new { HashedPassword = newHashedPassword, Id = id },
                transaction);

            if (rowsAffected > 1)
            {
                throw new InvalidOperationException($"Data integrity issue: {rowsAffected} rows updated for UserId {id}");
            }

            transaction.Commit();
            return rowsAffected == 1;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Rolling back password update for UserId {UserId}", id);
            transaction.Rollback();
            return false;
        }
    }
}
