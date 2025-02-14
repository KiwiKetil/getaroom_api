using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomSchedulerAPI.Features.Models.DTOs.Token;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Services;
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.Security.Claims;

namespace RoomSchedulerAPI.Features.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {   
        // https://localhost:7089/api/v1/users?page=1&pageSize=10     
        app.MapGet("/api/v1/users", async (IUserService userService, ILogger<Program> logger, [AsParameters] UserQuery query) => 
        {
            return await UserEndpointsLogic.GetAllUsersLogicAsync(userService, logger, query);
        })
        .RequireAuthorization("AdminAndPasswordUpdatedPolicy")
        .WithName("GetAllUsers");

        // https://localhost:7089/api/v1/users/887ac10b-58cc-4372-a567-0e02b2c3d493
        app.MapGet("/api/v1/users/{id}", async ([FromRoute] Guid id, IUserService userService, ILogger<Program> logger, ClaimsPrincipal claims) =>
        {
            return await UserEndpointsLogic.GetUserByIdLogicAsync(id, userService, logger, claims);        
        })
        .RequireAuthorization("PasswordUpdatedPolicy")
        .WithName("GetUserById");

        // https://localhost:7089/api/v1/users/b97ac10b-58cc-4372-a567-0e02b2c3d490
        app.MapPut("/api/v1/users/{id}", async ([FromRoute] Guid id, [FromBody] UserUpdateDTO dto, IUserService userService, IValidator<UserUpdateDTO> validator, ILogger<Program> logger, ClaimsPrincipal claims) =>
        {
            return await UserEndpointsLogic.UpdateUserLogicAsync(id, dto, userService, validator, logger, claims);
        })
        .RequireAuthorization("PasswordUpdatedPolicy")
        .WithName("UpdateUser");

        // https://localhost:7089/api/v1/users/6d7b1ca5-54f6-4859-a746-fc712d564128
        app.MapDelete("/api/v1/users/{id}", async ([FromRoute] Guid id, IUserService userService, ILogger<Program> logger) =>
        {
            return await UserEndpointsLogic.DeleteUserLogicAsync(id, userService, logger);
        })
        .RequireAuthorization("AdminAndPasswordUpdatedPolicy")
        .WithName("DeleteUser");

        // https://localhost:7089/api/v1/users/register
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
        .RequireAuthorization("AdminAndPasswordUpdatedPolicy")
        .WithName("RegisterUser");

        // https://localhost:7089/api/v1/login
        app.MapPost("/api/v1/login",
            static async ([FromBody] LoginDTO dto,
            IValidator<LoginDTO> validator,
            IUserService userService,
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

                bool hasUpdatedPassword = await userService.HasUpdatedPassword(authenticatedUser.Id);
                var token = await tokenGenerator.GenerateTokenAsync(authenticatedUser, hasUpdatedPassword);

                return Results.Ok(new TokenResponse { Token = token });
            })
        .WithName("UserLogin");

        // https://localhost:7089/api/v1/users/change-password
        app.MapPost("/api/v1/users/update-password", 
            static async ([FromBody] UpdatePasswordDTO dto,
            IValidator<UpdatePasswordDTO> validator,
            IUserService userService,
            ITokenGenerator tokenGenerator,
            ILogger<Program> logger,
            ClaimsPrincipal claims) =>
        {
                logger.LogDebug("User updating password");

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

                var passwordChanged = await userService.UpdatePasswordAsync(dto );
                if (!passwordChanged)
                { 
                    return Results.BadRequest(new { Message = "Password could not be updated. Please check your username or password and try again." });
                }

                var user = await userService.GetUserByEmailAsync(dto.Email);
                if (user is null)
                {
                    logger.LogError("User not found by email {Email}", dto.Email);
                    return Results.NotFound("User not found.");
                }

                var newToken = await tokenGenerator.GenerateTokenAsync(user, true);

                return Results.Ok(new TokenResponse { Token = newToken });
        })
        .RequireAuthorization()
        .WithName("UpdatePassword");
    }
}
