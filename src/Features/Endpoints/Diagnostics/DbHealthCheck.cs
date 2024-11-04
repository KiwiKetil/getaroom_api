using Microsoft.AspNetCore.Mvc;
using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using System.Data.Common;

namespace RoomSchedulerAPI.Features.Endpoints.Diagnostics;

public static class DBHealthCheck
{
    public static void MapDbHealthCheckEndpoint(this WebApplication app)
    {
        app.MapGet("/test-db-connection", async ([FromServices] IDbConnectionFactory connectionFactory) =>
        {
            using var connection = connectionFactory.CreateConnection() as DbConnection;
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
