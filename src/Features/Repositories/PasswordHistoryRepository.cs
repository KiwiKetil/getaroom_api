using Dapper;
using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;

namespace RoomSchedulerAPI.Features.Repositories;

public class PasswordHistoryRepository(IDbConnectionFactory mySqlConnectionFactory, ILogger<PasswordHistoryRepository> logger) : IPasswordHistoryRepository
{
    private readonly ILogger _logger = logger;
    private readonly IDbConnectionFactory _mySqlConnectionFactory = mySqlConnectionFactory;  

    public async Task<bool> PasswordChangeExistsAsync(UserId id)
    {
        _logger.LogDebug("Checking db if user has updated default password");

        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();

        string passwordChangeSql = @"SELECT COUNT(*) FROM PasswordHistory WHERE UserId = @id";

        int count = await dbConnection.ExecuteScalarAsync<int>(passwordChangeSql, new { id = id.Value });
        return count > 0;
    }

    public async Task InsertPasswordChangeRecordAsync(Guid userId)
    {
        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();
        string sql = @"
        INSERT INTO PasswordHistory (Id, UserId, ChangedDate)
        VALUES (@Id, @UserId, CURRENT_TIMESTAMP)
    ";
        await dbConnection.ExecuteAsync(sql, new { Id = Guid.NewGuid(), UserId = userId });
    }

}
