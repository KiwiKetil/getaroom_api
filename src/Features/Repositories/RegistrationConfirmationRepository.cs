using GetARoomAPI.Core.DB.DBConnection.Interface;
using GetARoomAPI.Core.DB.DBConnection;
using GetARoomAPI.Features.Models.Entities;
using GetARoomAPI.Features.Repositories.Interfaces;
using Dapper;

namespace GetARoomAPI.Features.Repositories;

public class RegistrationConfirmationRepository : IRegistrationConfirmationRepository
{
    private readonly IDbConnectionFactory _mySqlConnectionFactory;

    public RegistrationConfirmationRepository(IDbConnectionFactory mySqlConnectionFactory)
    {
        _mySqlConnectionFactory = mySqlConnectionFactory;
    }

    public async Task<UserRegistrationToken?> GetTokenAsync(string tokenString)
    {
        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();
        var sql = @"
        SELECT 
            CAST(Id AS CHAR(36)) AS Id, 
            UserId, 
            TokenString, 
            DateCreated, 
            DateExpired, 
            IsConfirmed 
        FROM UserRegistrationTokens 
        WHERE TokenString = @TokenString";
        return await dbConnection.QuerySingleOrDefaultAsync<UserRegistrationToken>(sql, new { TokenString = tokenString });
    }


    public async Task InsertTokenAsync(UserRegistrationToken token)
    {
        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();

        var sql = @"
        INSERT INTO UserRegistrationTokens 
            (Id, UserId, TokenString, DateCreated, DateExpired, IsConfirmed)
        VALUES 
            (@Id, @UserId, @TokenString, @DateCreated, @DateExpired, @IsConfirmed)";

        await dbConnection.ExecuteAsync(sql, token);
    }

    public async Task UpdateTokenAsync(UserRegistrationToken token)
    {
        using var dbConnection = await _mySqlConnectionFactory.CreateConnectionAsync();
        var sql = @"
        UPDATE UserRegistrationTokens 
        SET 
            UserId = @UserId, 
            TokenString = @TokenString, 
            DateCreated = @DateCreated, 
            DateExpired = @DateExpired, 
            IsConfirmed = @IsConfirmed
        WHERE Id = @Id";
        await dbConnection.ExecuteAsync(sql, token);
    }
}
