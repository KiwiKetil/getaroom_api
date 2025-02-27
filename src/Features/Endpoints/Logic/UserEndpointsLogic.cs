using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RoomSchedulerAPI.Features.Models.DTOs.ResponseDTOs;
using RoomSchedulerAPI.Features.Models.DTOs.TokenDTOs;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.Security.Claims;

namespace RoomSchedulerAPI.Features.Endpoints.Logic;

public static class UserEndpointsLogic
{
    public static async Task<IResult> GetUsersLogicAsync(
        [AsParameters] UserQuery query,
        IUserService userService,
        ILogger<Program> logger)
    {
        logger.LogDebug("Retrieving users");

        var usersAndCountDTO = await userService.GetUsersAsync(query);
        if (!usersAndCountDTO.Data.Any())
        {
            return Results.NotFound(new ErrorResponse(Message: "No users found"));
        }

        logger.LogDebug("Count is: {totalCount}", usersAndCountDTO.TotalCount);

        return Results.Ok(usersAndCountDTO);
    }

    public static async Task<IResult> GetUserByIdLogicAsync(
        [FromRoute] Guid id,
        IUserService userService,
        ClaimsPrincipal claims,
        ILogger<Program> logger)
    {
        logger.LogDebug("Retrieving user with ID {userId}", id);

        var isAdmin = claims.IsInRole("Admin");

        if (!isAdmin)
        {
            var userIdClaim = claims.FindFirst("sub") ?? claims.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || userIdClaim.Value != id.ToString() || !claims.IsInRole("User"))
            {
                return Results.Forbid();
            }
        }

        var userDTO = await userService.GetUserByIdAsync(id);
        return userDTO != null ? Results.Ok(userDTO)
            : Results.NotFound(new ErrorResponse(Message: "User was not found"));
    }

    public static async Task<IResult> UpdateUserLogicAsync(
        [FromRoute] Guid id,
        [FromBody] UserUpdateDTO dto,
        IUserService userService,
        IValidator<UserUpdateDTO> validator,
        ClaimsPrincipal claims,
        ILogger<Program> logger)
    {
        logger.LogDebug("Updating user with ID {userId}", id);

        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Results.BadRequest( new ErrorResponse ( Errors: errors ));
        }

        var isAdmin = claims.IsInRole("Admin");
        if (!isAdmin)
        {
            var userIdClaim = claims.FindFirst("sub") ?? claims.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || userIdClaim.Value != id.ToString() || !claims.IsInRole("User"))
            {
                return Results.Forbid();
            }
        }

        var userDTO = await userService.UpdateUserAsync(id, dto);
        return userDTO != null ? Results.Ok(userDTO)
            : Results.Conflict(new ErrorResponse(Message: "User could not be updated"));
    }

    public static async Task<IResult> DeleteUserLogicAsync(
        [FromRoute] Guid id,
        IUserService userService,
        ILogger<Program> logger)
    {
        logger.LogDebug("Deleting user with ID {userId}", id);

        var userDTO = await userService.DeleteUserAsync(id);
        return userDTO != null ? Results.Ok(userDTO)
            : Results.Conflict(new ErrorResponse(Message: "User could not be deleted"));
    }

    public static async Task<IResult> RegisterUserLogicAsync(
        [FromBody] UserRegistrationDTO dto,
        IValidator<UserRegistrationDTO> validator,
        IUserService userService,
        ILogger<Program> logger)
    {
        logger.LogDebug("Registering new user");

        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Results.BadRequest(new ErrorResponse (Errors: errors));
        }

        var userDTO = await userService.RegisterUserAsync(dto);

        return userDTO != null ? Results.Ok(userDTO)
            : Results.Conflict(new ErrorResponse(Message: "User already exists"));
    }

    public static async Task<IResult> UserLoginLogicAsync(
        [FromBody] LoginDTO dto,
        IValidator<LoginDTO> validator,
        IUserService userService,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IPasswordVerificationService passwordVerificationService,
        ITokenGenerator tokenGenerator,
        ILogger<Program> logger)
    {
        logger.LogDebug("User logging in");

        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Results.BadRequest(new ErrorResponse( Errors: errors ));
        }

        var user = await userRepository.GetUserByEmailAsync(dto.Email);
        if (user == null)
        {
            return Results.NotFound(new ErrorResponse(Message: "User not found"));
        }

        var verifiedUser = passwordVerificationService.VerifyPassword(dto, user);
        if (!verifiedUser)
        {
            return Results.Unauthorized();
        }

        bool hasUpdatedPassword = await userService.HasUpdatedPassword(user.Id);
        var userRoles = await userRoleRepository.GetUserRoles(user.Id);

        var token = tokenGenerator.GenerateToken(user, hasUpdatedPassword, userRoles);

        return Results.Ok(new TokenResponse { Token = token });
    }

    public static async Task<IResult> UpdatePasswordLogicAsync(
        [FromBody] UpdatePasswordDTO dto,
        IValidator<UpdatePasswordDTO> validator,
        IUserService userService,
        IUserRoleRepository userRoleRepository,
        IPasswordVerificationService passwordVerificationService,
        ITokenGenerator tokenGenerator,
        ClaimsPrincipal claims,
        ILogger<Program> logger)
    {
        logger.LogDebug("User updating password");

        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return Results.BadRequest(new ErrorResponse(Errors: errors));
        }

        var userIdClaim = claims.FindFirst("name") ?? claims.FindFirst(ClaimTypes.Name);
        if (userIdClaim == null || userIdClaim.Value != dto.Email || !claims.IsInRole("User"))
        {
            return Results.Forbid();
        }

        var user = await userService.GetUserByEmailAsync(dto.Email);
        if (user is null)
        {
            logger.LogError("User not found by email {Email}", dto.Email);
            return Results.NotFound(new ErrorResponse(Message: "User not found"));
        }

        var verifiedUser = passwordVerificationService.VerifyPassword(dto, user);
        if (!verifiedUser)
        {
            return Results.Unauthorized();
        }

        var passwordChanged = await userService.UpdatePasswordAsync(dto, user);
        if (!passwordChanged)
        {
            return Results.BadRequest(new ErrorResponse(Message: "Password could not be updated. Please check your username or password and try again."));
        }
        var userRoles = await userRoleRepository.GetUserRoles(user.Id);
        var newToken = tokenGenerator.GenerateToken(user, true, userRoles);

        return Results.Ok(new TokenResponse { Token = newToken });
    }
}
