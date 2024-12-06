
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
            logger.LogDebug("Retrieving all users");

            var users = await userService.GetAllUsersAsync(page, pageSize);
            return users.Any() ? Results.Ok(users) : Results.NotFound("No users found");
        })
        .WithName("GetAllUsers"); ;


        app.MapGet("/api/v1/users/{id}", async ([FromRoute] Guid id, IUserService userService, ILogger<Program> logger) =>
        {
            logger.LogDebug("Retrieving user with ID {userId}", id);

            var user = await userService.GetUserByIdAsync(id);
            return user != null ? Results.Ok(user) : Results.NotFound("User was not found");
        })
        .WithName("GetUserById"); ;


        app.MapPut("/api/v1/users/{id}", async ([FromRoute] Guid id, [FromBody] UserUpdateDTO dto, IUserService userService, IValidator<UserUpdateDTO> validator, ILogger<Program> logger) =>
        {
            logger.LogDebug("Updating user with ID {userId}", id);

            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Results.BadRequest(errors);
            }

            var user = await userService.UpdateUserAsync(id, dto);
            return user != null ? Results.Ok(user) : Results.Problem(
                title: "An issue occured",
                statusCode: 409,
                detail: "User could not be updated"
                );
        })
        .WithName("UpdateUser"); ;


        app.MapDelete("/api/v1/users/{id}", async ([FromRoute] Guid id, IUserService userService, ILogger<Program> logger) =>
        {
            logger.LogDebug("Deleting user with ID {userId}", id);

            var user = await userService.DeleteUserAsync(id);
            return user != null ? Results.Ok(user) : Results.Problem(
                title: "An issue occured",
                statusCode: 409,
                detail: "User could not be deleted"
                );
        })
        .WithName("DeleteUser");

        //register(?)
        //app.MapGet("/api/v1/users", async (IUserService userService) =>
        //{
        //    var users = await userService.GetAllAsync();
        //    return Results.Ok(users);
        //});
    }
}
