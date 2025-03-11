using GetARoomAPI.Core.DB.DBConnection.Interface;
using MySql.Data.MySqlClient;
using System.Data;

namespace GetARoomAPI.Core.DB.DBConnection;

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
