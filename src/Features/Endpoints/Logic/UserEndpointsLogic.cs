using GetARoomAPI.Features.Models.DTOs.ResponseDTOs;
using GetARoomAPI.Features.Models.DTOs.UserDTOs;
using GetARoomAPI.Features.Models.Enums;
using GetARoomAPI.Features.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GetARoomAPI.Features.Endpoints.Logic;

public static class UserEndpointsLogic
{
    public static async Task<IResult> GetUsersLogicAsync(
        [AsParameters] UserQuery query,
        IUserService userService,
        ILogger<Program> logger)
    {
        logger.LogDebug("Retrieving users");

        var usersAndCountDTO = await userService.GetUsersAsync(query);
        if (!usersAndCountDTO.UserDTOs.Any())
        {
            return Results.NotFound(new ErrorResponse(Message: "No users found"));
        }

        return Results.Ok(usersAndCountDTO);
    }

    public static async Task<IResult> GetUserByIdLogicAsync(
        [FromRoute] Guid id,
        IUserService userService,
        ILogger<Program> logger)
    {
        logger.LogDebug("Retrieving user with ID {userId}", id);

        var userDTO = await userService.GetUserByIdAsync(id);
        return userDTO != null
            ? Results.Ok(userDTO)
            : Results.NotFound(new ErrorResponse(Message: "User was not found"));
    }

    public static async Task<IResult> UpdateUserLogicAsync(
        [FromBody] UserUpdateDTO dto,
        [FromRoute] Guid id,
        IUserService userService,
        IAuthorizationService authorizationService,
        ClaimsPrincipal claims,
        ILogger<Program> logger)
    {
        logger.LogDebug("Updating user with ID {userId}", id);

        var authorizationResult = await authorizationService.AuthorizeAsync(claims, id, "UserIdAccessPolicy");
        if (!authorizationResult.Succeeded)
        {
            return Results.Forbid();
        }

        var userDTO = await userService.UpdateUserAsync(id, dto);
        return userDTO != null
            ? Results.Ok(userDTO)
            : Results.Conflict(new ErrorResponse(Message: "User could not be updated"));
    }

    public static async Task<IResult> DeleteUserLogicAsync(
        [FromRoute] Guid id,
        IUserService userService,
        ILogger<Program> logger)
    {
        logger.LogDebug("Deleting user with ID {userId}", id);

        var userDTO = await userService.DeleteUserAsync(id);
        return userDTO != null
            ? Results.Ok(userDTO)
            : Results.Conflict(new ErrorResponse(Message: "User could not be deleted"));
    }

    public static async Task<IResult> RegisterEmployeeLogicAsync(
        [FromBody] UserRegistrationDTO dto,
        IUserService userService,
        ILogger<Program> logger)
    {
        logger.LogDebug("Registering new employee");

        var userDTO = await userService.RegisterUserAsync(dto, UserRoles.Employee);
        return userDTO != null
            ? Results.Ok(userDTO)
            : Results.Conflict(new ErrorResponse(Message: "Employee could not be registered"));
    }

    public static async Task<IResult> RegisterClientLogicAsync(
        [FromBody] UserRegistrationDTO dto,
        IUserService userService,
        ILogger<Program> logger)
    {
        logger.LogDebug("Registering new client");

        var userDTO = await userService.RegisterUserAsync(dto, UserRoles.Client);
        return userDTO != null
            ? Results.Ok(userDTO)
            : Results.Conflict(new ErrorResponse(Message: "Client could not be registered"));
    }

    public static async Task<IResult> UserLoginLogicAsync(
        [FromBody] LoginDTO dto,
        IUserService userService,
        ILogger<Program> logger)
    {
        logger.LogDebug("User logging in");

        var token = await userService.UserLoginAsync(dto);
        return token != null
            ? Results.Ok(new TokenResponse(Token: token))
            : Results.Unauthorized();
    }

    public static async Task<IResult> UpdatePasswordLogicAsync(
        [FromBody] UpdatePasswordDTO dto,
        IUserService userService,
        IAuthorizationService authorizationService,
        ClaimsPrincipal claims,
        ILogger<Program> logger)
    {
        logger.LogDebug("User updating password");

        var authorizationResult = await authorizationService.AuthorizeAsync(claims, dto, "UserNameAccessPolicy");
        if (!authorizationResult.Succeeded)
        {
            return Results.Forbid();
        }

        var res = await userService.UpdatePasswordAsync(dto);

        return res
            ? Results.Ok( new OkResponse(Message: "Password was updated"))
            : Results.Unauthorized();
    }

    public static async Task<IResult> ConfirmRegistrationAsync(
    [FromQuery] string token,
    IRegistrationConfirmationService registrationConfirmationService,
    ILogger<Program> logger)
    {
        logger.LogDebug("User confirming registration");

        token = token.Trim();
        var res = await registrationConfirmationService.ConfirmRegistrationAsync(token);

        return res
            ? Results.Ok()
            : Results.Conflict("Something went wrong"); // temp msg
    }
}
