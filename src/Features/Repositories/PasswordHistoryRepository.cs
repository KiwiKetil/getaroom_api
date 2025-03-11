using Dapper;
using GetARoomAPI.Core.DB.DBConnection.Interface;
using GetARoomAPI.Core.DB.UnitOFWork.Interfaces;
using GetARoomAPI.Features.Models.Entities;
using GetARoomAPI.Features.Repositories.Interfaces;

namespace GetARoomAPI.Features.Repositories;

public class PasswordHistoryRepository(IDbConnectionFactory mySqlConnectionFactory, ILogger<PasswordHistoryRepository> logger) : IPasswordHistoryRepository
{
    private readonly ILogger _logger = logger;
    private readonly IDbConnectionFactory _mySqlConnectionFactory = mySqlConnectionFactory;

    public async Task<bool> PasswordUpdateExistsAsync(UserId id)
    {
        _logger.LogDebug("Checking db if user has updated default password");

        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();

        string passwordChangeSql = @"SELECT COUNT(*) FROM PasswordHistory WHERE UserId = @id";

        int count = await dbConnection.ExecuteScalarAsync<int>(passwordChangeSql, new { id = id.Value });
        return count > 0;
    }

    public async Task<bool> InsertPasswordUpdateRecordAsync(Guid userId, IUnitOfWork unitOfWork)
    {
        string sql = @"
        INSERT INTO PasswordHistory (Id, UserId, ChangedDate)
                     VALUES (@Id, @UserId, CURRENT_TIMESTAMP)";

        int rowsAffected = await unitOfWork.Connection.ExecuteAsync(sql, new { Id = Guid.NewGuid(), UserId = userId }, unitOfWork.Transaction);

        if (rowsAffected != 1)
        {
            throw new InvalidOperationException($"Expected to affect 1 row, but affected {rowsAffected} rows for UserId {userId}");
        }

        return true;
    }
}
