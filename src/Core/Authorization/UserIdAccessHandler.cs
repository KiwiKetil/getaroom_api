using GetARoomAPI.Features.Models.Entities;
using GetARoomAPI.Features.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GetARoomAPI.Core.Authorization;

public class UserIdAccessHandler : AuthorizationHandler<UserIdAccessRequirement, Guid> 
{
    private readonly IUserRoleRepository _userRoleRepository;
    public UserIdAccessHandler(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                    UserIdAccessRequirement requirement,
                                                    Guid resource)
    {
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        if (context.User.IsInRole("Employee"))
        {
            var userRoles = await _userRoleRepository.GetUserRolesAsync(new UserId(resource));

            if (userRoles.Any(r => r.RoleName == "Client"))
            {
                context.Succeed(requirement);
                return;
            }
        }

        var userIdClaim = context.User.FindFirst("sub") ?? context.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null && userIdClaim.Value == resource.ToString())
        {
            context.Succeed(requirement);
        }
        return;
    }
}
     