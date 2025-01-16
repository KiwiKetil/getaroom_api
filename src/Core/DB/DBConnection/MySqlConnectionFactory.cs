using MySql.Data.MySqlClient;
using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using System.Data;

namespace RoomSchedulerAPI.Core.DB.DBConnection;

public class MySqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    private readonly string _connectionString = connectionString;

    public IDbConnection CreateConnection()
    {
        var connection = new MySqlConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(); 
        return connection;
    }
}
