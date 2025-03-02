using Microsoft.AspNetCore.Authorization;
using RoomSchedulerAPI.Core.Authorization;

namespace RoomSchedulerAPI.Core.Extensions;

public static class AuthorizationPoliciesExtensions
{
    public static AuthorizationBuilder AddCustomPolicies(this AuthorizationBuilder builder)
    {
        return builder
            .AddPolicy("PasswordUpdatedPolicy", policy =>
            {
                policy.RequireClaim("passwordUpdated", "true");
            })

            .AddPolicy("AdminAndPasswordUpdatedPolicy", policy =>
            {
                policy.RequireRole("Admin");
                policy.RequireClaim("passwordUpdated", "true");
            })

            .AddPolicy("UserIdAccessPolicy", policy =>
            policy.Requirements.Add(new UserIdAccessRequirement()))

            .AddPolicy("UserNameAccessPolicy", policy =>
            policy.Requirements.Add(new UserNameAccessRequirement())); ;
    }
}
