using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RoomSchedulerAPI.Features.Models.DTOs.Token;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.Security.Claims;

namespace RoomSchedulerAPI.Features.Endpoints;

public static class UserEndpointsLogic
{
    public static async Task<IResult> GetAllUsersLogicAsync(IUserService userService, UserQuery query, ILogger logger)
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
    }

    public static async Task<IResult> GetUserByIdLogicAsync([FromRoute] Guid id, IUserService userService, ClaimsPrincipal claims, ILogger<Program> logger)
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
    }

    public static async Task<IResult> UpdateUserLogicAsync([FromRoute] Guid id, [FromBody] UserUpdateDTO dto, IUserService userService,
            IValidator<UserUpdateDTO> validator,
            ClaimsPrincipal claims,
            ILogger<Program> logger)
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
    }

    public static async Task<IResult> DeleteUserLogicAsync([FromRoute] Guid id, IUserService userService, ILogger<Program> logger)
    {
        logger.LogDebug("Deleting user with ID {userId}", id);

        var user = await userService.DeleteUserAsync(id);
        return user != null ? Results.Ok(user) : Results.Problem(
            title: "An issue occured",
            statusCode: 409,
            detail: "User could not be deleted"
        );
    }

    public static async Task<IResult> RegisterUserLogicAsync([FromBody] UserRegistrationDTO dto, IValidator<UserRegistrationDTO> validator, IUserService userService, ILogger<Program> logger)
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

    }

    public static async Task<IResult> UserLoginLogicAsync([FromBody] LoginDTO dto, IValidator<LoginDTO> validator, IUserService userService, IUserAuthenticationService authService, ITokenGenerator tokenGenerator, ILogger<Program> logger)
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
    }

    public static async Task<IResult> UpdatePasswordLogicAsync([FromBody] UpdatePasswordDTO dto,
            IValidator<UpdatePasswordDTO> validator,
            IUserService userService,
            ITokenGenerator tokenGenerator,
            ClaimsPrincipal claims,
            ILogger<Program> logger
)
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

        var passwordChanged = await userService.UpdatePasswordAsync(dto);
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
    }
}
