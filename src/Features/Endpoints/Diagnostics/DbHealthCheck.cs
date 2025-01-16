using Microsoft.AspNetCore.Mvc;
using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using System.Data.Common;

namespace RoomSchedulerAPI.Features.Endpoints.Diagnostics;

public static class DBHealthCheck
{
    public static void MapDbHealthCheckEndpoint(this WebApplication app)
    {
        // Kun admin
        app.MapGet("/test-db-connection", async ([FromServices] IDbConnectionFactory connectionFactory) =>
        {
            try
            {
                using var syncConnection = connectionFactory.CreateConnection() as DbConnection;
                if (syncConnection == null)
                {
                    return Results.Problem("Failed to create a synchronous database connection.");
                }

                syncConnection.Close();

                using var asyncConnection = await connectionFactory.CreateConnectionAsync() as DbConnection;
                if (asyncConnection == null)
                {
                    return Results.Problem("Failed to create a asynchronous database connection.");
                }
           
                return Results.Ok("Both synchronous and asynchronous connections was successful!");                
            }
            catch (Exception ex)
            {
                return Results.Problem($"Connection failed: {ex.Message}");
            }
        });
    }
}
