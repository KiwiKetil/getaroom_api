using GetARoomAPI.Core.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GetARoomAPI.Core.Extensions;

public static class AuthorizationPoliciesExtensions
{
    public static AuthorizationBuilder AddCustomPolicies(this AuthorizationBuilder builder)
    {
        return builder
            // endpoint policies
            .AddPolicy("EmployeeOrAdminWithUpdatedPasswordPolicy", policy =>
            {
                policy.RequireRole("Employee", "Admin");
                policy.RequireClaim("passwordUpdated", "true");
            })

            .AddPolicy("AdminWithUpdatedPasswordPolicy", policy =>
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
