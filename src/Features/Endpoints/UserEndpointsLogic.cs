using Microsoft.AspNetCore.Mvc;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.Security.Claims;

namespace RoomSchedulerAPI.Features.Endpoints;

public static class UserEndpointsLogic
{
    public static async Task<IResult> GetAllUsersLogicAsync(IUserService userService, ILogger logger, UserQuery query)
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

    public static async Task<IResult> GetUserByIdLogicAsync([FromRoute] Guid id, IUserService userService, ILogger<Program> logger, ClaimsPrincipal claims)
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
}
