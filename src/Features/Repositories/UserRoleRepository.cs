using Dapper;
using GetARoomAPI.Core.DB.DBConnection.Interface;
using GetARoomAPI.Core.DB.UnitOFWork.Interfaces;
using GetARoomAPI.Features.Models.Entities;
using GetARoomAPI.Features.Models.Enums;
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

    public async Task<bool> AddUserRoleAsync(UserId id, UserRoles role, IUnitOfWork unitOfWork)
    {
        _logger.LogDebug("Adding role for user with ID: {UserId}", id);

        string addRoleSql = @"INSERT INTO UserRoles (UserId, RoleName) VALUES (@Id, @Role)";

        int rowsAffected = await unitOfWork.Connection.ExecuteAsync(addRoleSql, new { Id = id.Value, Role = role.ToString() }, unitOfWork.Transaction); // tostting needed on role?

        if (rowsAffected > 1)
        {
            throw new InvalidOperationException($"Data integrity issue: {rowsAffected} rows updated for UserId {id}");
        }

        return rowsAffected == 1;
    }
}
