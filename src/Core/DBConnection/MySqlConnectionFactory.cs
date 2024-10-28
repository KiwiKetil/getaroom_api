using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;

namespace RoomSchedulerAPI.Core.DBConnection;

public class MySqlConnectionFactory(string connectionString)
{
    private readonly string _connectionString = connectionString;

    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
