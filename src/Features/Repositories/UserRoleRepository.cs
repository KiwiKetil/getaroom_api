using Dapper;
using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;

namespace RoomSchedulerAPI.Features.Repositories;

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
