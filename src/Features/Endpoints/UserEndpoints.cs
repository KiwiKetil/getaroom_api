
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.Data;

namespace RoomSchedulerAPI.Features.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/v1/users", async (IUserService userService, ILogger<Program> logger) =>
        {
            logger.LogInformation("Retrieving all users");
            var users = await userService.GetAllAsync(); 
            return Results.Ok(users);
        });

        //app.MapGet("/api/v1/users/{id}", async (IUserService userService, Guid id) =>
        //{
        //    var users = await userService.GetByIdAsync(new UserId(id));
        //    return Results.Ok(users);
        //});

        //app.MapGet("/api/v1/users", async (IUserService userService) =>
        //{
        //    var users = await userService.GetAllAsync();
        //    return Results.Ok(users);
        //});

        //app.MapGet("/api/v1/users", async (IUserService userService) =>
        //{
        //    var users = await userService.GetAllAsync();
        //    return Results.Ok(users);
        //});

        //app.MapGet("/api/v1/users", async (IUserService userService) =>
        //{
        //    var users = await userService.GetAllAsync();
        //    return Results.Ok(users);
        //});
    }
}
