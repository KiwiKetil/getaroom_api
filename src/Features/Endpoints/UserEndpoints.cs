using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.Security.Claims;

namespace RoomSchedulerAPI.Features.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        // admin only
        app.MapGet("/api/v1/users",
            static async (IUserService userService, 
            ILogger<Program> logger,
            [AsParameters] UserQuery query) =>
        {
                logger.LogDebug("Retrieving users");

                var (users, totalCount) = await userService.GetUsersAsync(query);
                if (!users.Any())
                {
                    return Results.NotFound("No users found");
                }

                logger.LogDebug("Count is: {totalCount}", totalCount);

                return Results.Ok(new
                {
                    TotalCount = totalCount,
                    Data = users
                });
        })
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
        .WithName("GetAllUsers");
        
        app.MapGet("/api/v1/users/{id}", 
            static async ([FromRoute] Guid id,
            IUserService userService,
            ILogger<Program> logger,
            ClaimsPrincipal claims) =>
        {
                logger.LogDebug("Retrieving user with ID {userId}", id);

                var isAdmin = claims.IsInRole("Admin");

                if (!isAdmin)
                {
                    var userIdClaim = claims.FindFirst("sub") ?? claims.FindFirst(ClaimTypes.NameIdentifier);

                    if (userIdClaim == null || userIdClaim.Value != id.ToString())
                    {
                        return Results.Forbid();
                    }
                }

                var user = await userService.GetUserByIdAsync(id);
                return user != null ? Results.Ok(user) : Results.NotFound("User was not found");
        })
        .RequireAuthorization()
        .WithName("GetUserById");

        app.MapPut("/api/v1/users/{id}", 
            static async ([FromRoute] Guid id,             
            [FromBody] UserUpdateDTO dto,
            IUserService userService, 
            IValidator<UserUpdateDTO> validator, 
            ILogger<Program> logger,
            ClaimsPrincipal claims) =>
        {
                logger.LogDebug("Updating user with ID {userId}", id);

                var isAdmin = claims.IsInRole("Admin");

                if (!isAdmin)
                {
                    var userIdClaim = claims.FindFirst("sub") ?? claims.FindFirst(ClaimTypes.NameIdentifier);

                    if (userIdClaim == null || userIdClaim.Value != id.ToString())
                    {
                        return Results.Forbid();
                    }
                }

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
        .RequireAuthorization()
        .WithName("UpdateUser");

        app.MapDelete("/api/v1/users/{id}", 
            static async ([FromRoute] Guid id,
            IUserService userService, 
            ILogger<Program> logger) =>
        {
                logger.LogDebug("Deleting user with ID {userId}", id);

                var user = await userService.DeleteUserAsync(id);
                return user != null ? Results.Ok(user) : Results.Problem(
                    title: "An issue occured",
                    statusCode: 409,
                    detail: "User could not be deleted"
                );
        })
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
        .WithName("DeleteUser");

        app.MapPost("/api/v1/users/register", 
            static async ([FromBody] UserRegistrationDTO dto, 
            IValidator<UserRegistrationDTO> validator, 
            IUserService userService, 
            ILogger<Program> logger) =>
        {
                logger.LogDebug("Registering new user");

                var validationResult = await validator.ValidateAsync(dto);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return Results.BadRequest(errors);
                }

                var res = await userService.RegisterUserAsync(dto);

                return res != null ? Results.Ok(res) : Results.Conflict(new { Message = "User already exists" });
        })
        .WithName("RegisterUser")
        .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });


        // new users must change passwordgiven by admin within(?)
        app.MapPost("/api/v1/users/change-password", 
            static async ([FromBody] ChangePasswordDTO dto,
            IValidator<ChangePasswordDTO> validator,
            IUserService userService,
            ILogger<Program> logger,
            ClaimsPrincipal claims) =>
        {
                logger.LogDebug("User changing password");

                var isAdmin = claims.IsInRole("Admin");

                if (!isAdmin)
                {
                    var userIdClaim = claims.FindFirst("name") ?? claims.FindFirst(ClaimTypes.Name);

                    if (userIdClaim == null || userIdClaim.Value != dto.Email)
                    {
                        return Results.Forbid();
                    }
                }

                var validationResult = await validator.ValidateAsync(dto);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return Results.BadRequest(errors);
                }

                var res = await userService.ChangePasswordAsync(dto);

                return res
                ? Results.Ok(new { Message = "Password changed successfully." })
                : Results.BadRequest(new { Message = "Password could not be changed. Please check your username or password and try again." });
        })
        .RequireAuthorization()
        .WithName("ChangePassword");

        app.MapPost("/api/v1/login", 
            static async ([FromBody] LoginDTO dto, 
            IValidator<LoginDTO> validator, 
            IUserAuthenticationService authService,
            ITokenGenerator tokenGenerator, 
            ILogger<Program> logger) =>
        {
                logger.LogDebug("User logging in");

                var validationResult = await validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return Results.BadRequest(errors);
                }

                var authenticatedUser = await authService.AuthenticateUserAsync(dto);

                if (authenticatedUser == null)
                {
                    return Results.Problem("Login failed. Please check your username and/or password and try again.", statusCode: 401);
                }

                var token = await tokenGenerator.GenerateTokenAsync(authenticatedUser);

                return Results.Ok(new { Token = token });
        })
        .WithName("UserLogin");
    }
}
