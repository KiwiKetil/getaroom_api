
using FluentValidation;
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
        })
        .WithName("GetAllUsers"); ;

        app.MapGet("/api/v1/users/{id}", async ([FromRoute] Guid id, IUserService userService, ILogger < Program > logger) =>
        {
            logger.LogInformation("Retrieving user with ID {userId}", id);

            var user = await userService.GetByIdAsync(id);
            return user != null ? Results.Ok(user) : Results.NotFound("User was not found");
        })
        .WithName("GetUserById"); ;

        app.MapPut("/api/v1/users/{id}", async ([FromRoute] Guid id, [FromBody] UserUpdateDTO dto, IUserService userService, IValidator <UserUpdateDTO> validator, ILogger <Program> logger) =>
        {
            logger.LogInformation("Updating user with ID {userId}", id);

            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Results.BadRequest(errors);
            }

            var user = await userService.UpdateAsync(id, dto);
            return user != null ? Results.Ok(user) : Results.Problem("User could not be updated");
        })
        .WithName("UpdateUser"); ;

        app.MapDelete("/api/v1/users/{id}", async ([FromRoute] Guid id, IUserService userService, ILogger<Program> logger) =>
        {
            logger.LogInformation("Deleting user with ID {userId}", id);

            var user = await userService.DeleteAsync(id);
            return user != null ? Results.Ok(user) : Results.Problem("User could not be deleted");
        })
        .WithName("DeleteUser");

        //app.MapGet("/api/v1/users", async (IUserService userService) =>
        //{
        //    var users = await userService.GetAllAsync();
        //    return Results.Ok(users);
        //});
    }
}
