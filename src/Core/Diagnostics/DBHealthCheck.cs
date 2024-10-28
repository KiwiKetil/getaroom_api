using MySql.Data.MySqlClient;
using RoomSchedulerAPI.Core.DbConnectionFactory;


namespace RoomSchedulerAPI.Core.Diagnostics;

public static class DBHealthCheck
{
    public static void MapDBHealthCheckEndpoint(this WebApplication app)
    {
        app.MapGet("/test-connection", async (MySqlConnectionFactory connectionFactory) =>
        {
            using var connection = connectionFactory.CreateConnection() as MySqlConnection;
            if (connection == null)
            {
                return Results.Problem("Failed to create a database connection.");
            }

            try
            {
                await connection.OpenAsync();
                return Results.Ok("Connection successful!");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Connection failed: {ex.Message}");
            }
        });
    }
}
