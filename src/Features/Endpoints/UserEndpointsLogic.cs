using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Services.Interfaces;

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
}
