using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GetARoomAPI.Core.Authorization;

public class UserIdAccessHandler : AuthorizationHandler<UserIdAccessRequirement, Guid> // This handler/ requirement might be obsolete after changes/ update to getARoomapi
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                    UserIdAccessRequirement requirement,
                                                    Guid resource)
    {
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var userIdClaim = context.User.FindFirst("sub") ?? context.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null && userIdClaim.Value == resource.ToString())
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
