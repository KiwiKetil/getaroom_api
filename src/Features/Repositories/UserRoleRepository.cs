using Dapper;
using GetARoomAPI.Core.DB.DBConnection.Interface;
using GetARoomAPI.Features.Models.Entities;
using GetARoomAPI.Features.Repositories.Interfaces;

namespace GetARoomAPI.Features.Repositories;

public class UserRoleRepository(IDbConnectionFactory mySqlConnectionFactory, ILogger<UserRoleRepository> logger) : IUserRoleRepository
{
    private readonly IDbConnectionFactory _mySqlConnectionFactory = mySqlConnectionFactory;
    private readonly ILogger _logger = logger;

    public async Task<IEnumerable<UserRole>> GetUserRolesAsync(UserId id)
    {
        _logger.LogDebug("Retrieving roles for user with ID: {UserId}", id);

        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();

        string getUserRolesSql = @"SELECT Rolename FROM UserRoles WHERE UserId = @id";
        return await dbConnection.QueryAsync<UserRole>(getUserRolesSql, new { id = id.Value });
    }
}
