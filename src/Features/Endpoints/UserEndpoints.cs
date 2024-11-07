
using Microsoft.AspNetCore.Mvc;
using RoomSchedulerAPI.Features.Models.DTOs;
using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.Features.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/v1/users", async (IUserService userService, ILogger<Program> logger, int page = 1, int pageSize = 10) =>
        {
            logger.LogInformation("Retrieving all users");

            var users = await userService.GetAllAsync(page, pageSize); 
            return users.Any() ? Results.Ok(users) : Results.NotFound("Could not find any users");
        });

        app.MapGet("/api/v1/users/{id}", async ([FromRoute] Guid id, IUserService userService, ILogger < Program > logger) =>
        {
            logger.LogInformation("Retrieving user with ID {user}", id);

            var user = await userService.GetByIdAsync(id);
            return user != null ? Results.Ok(user) : Results.NotFound("User was not found");
        });

        app.MapPut("/api/v1/users/{id}", async ([FromRoute] Guid id, [FromBody] UserUpdateDTO dto, IUserService userService, ILogger<Program> logger) =>
        {
            logger.LogInformation("Updating user with ID {userId}", id);

            var user = await userService.UpdateAsync(id, dto);
            return user != null ? Results.Ok(user) : Results.Problem("User could not be updated");
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
    }
}
