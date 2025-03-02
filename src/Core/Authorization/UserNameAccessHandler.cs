using Microsoft.AspNetCore.Authorization;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using System.Security.Claims;

namespace RoomSchedulerAPI.Core.Authorization;

public class UserNameAccessHandler : AuthorizationHandler<UserNameAccessRequirement, UpdatePasswordDTO>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                    UserNameAccessRequirement requirement,
                                                    UpdatePasswordDTO resource)
    {
        var userNameClaim = context.User.FindFirst("name") ?? context.User.FindFirst(ClaimTypes.Name);
        if (userNameClaim != null && userNameClaim.Value == resource.Email && context.User.IsInRole("User"))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
