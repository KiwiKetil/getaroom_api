
using Microsoft.AspNetCore.Mvc;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.Data;

namespace RoomSchedulerAPI.Features.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/v1/users", async (IUserService userService, ILogger<Program> logger, int page = 1, int pageSize = 10) =>
        {
            logger.LogInformation("Retrieving all users");

            var users = await userService.GetAllAsync(page, pageSize); 
            //return Results.Ok(users);
            return users.Any() ? Results.Ok(users) : Results.NotFound("Could not find any users");
        });

        app.MapGet("/api/v1/users/{id}", async (IUserService userService, ILogger < Program > logger, [FromRoute]Guid id) =>
        {
            logger.LogInformation("Retrieving {user}", id);

            var user = await userService.GetByIdAsync(id);
            return user != null ? Results.Ok(user) : Results.NotFound("User not found");
        });

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
