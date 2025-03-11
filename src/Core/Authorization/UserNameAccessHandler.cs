using GetARoomAPI.Features.Models.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GetARoomAPI.Core.Authorization;

public class UserNameAccessHandler : AuthorizationHandler<UserNameAccessRequirement, UpdatePasswordDTO>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                    UserNameAccessRequirement requirement,
                                                    UpdatePasswordDTO resource)
    {
        var userNameClaim = context.User.FindFirst("name") ?? context.User.FindFirst(ClaimTypes.Name);
        if (userNameClaim != null && userNameClaim.Value == resource.Email)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
