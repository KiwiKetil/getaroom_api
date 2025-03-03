using Microsoft.AspNetCore.Authorization;
using RoomSchedulerAPI.Core.Authorization;

namespace RoomSchedulerAPI.Core.Extensions;

public static class AuthorizationPoliciesExtensions
{
    public static AuthorizationBuilder AddCustomPolicies(this AuthorizationBuilder builder)
    {
        return builder
            // endpoint policies
            .AddPolicy("UserRoleAndPasswordUpdatedPolicy", policy =>
            {
                policy.RequireRole("User", "Admin");
                policy.RequireClaim("passwordUpdated", "true");
            })

            .AddPolicy("AdminRoleAndPasswordUpdatedPolicy", policy =>
            {
                policy.RequireRole("Admin");
                policy.RequireClaim("passwordUpdated", "true");
            })

            // authorization policies
            .AddPolicy("UserIdAccessPolicy", policy =>
            policy.Requirements.Add(new UserIdAccessRequirement()))

            .AddPolicy("UserNameAccessPolicy", policy =>
            policy.Requirements.Add(new UserNameAccessRequirement())); ;
    }
}
