using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;

namespace RoomSchedulerAPI.Core.Dapper;

public class MySqlConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(string connectionString)
    {

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string cannot be null or empty.");            
        }

        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
