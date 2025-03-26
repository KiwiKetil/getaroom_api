using GetARoomAPI.Core.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GetARoomAPI.Core.Extensions;

public static class AuthorizationPoliciesExtensions
{
    public static AuthorizationBuilder AddCustomPolicies(this AuthorizationBuilder builder)
    {
        return builder
            // endpoint policies
            .AddPolicy("EmployeeOrAdminWithConfirmedRegistrationPolicy", policy =>
            {
                policy.RequireRole("Employee", "Admin");
                policy.RequireClaim("hasConfirmedRegistration", "true");
            })

            .AddPolicy("AdminWithConfirmedRegistrationPolicy", policy =>
            {
                policy.RequireRole("Admin");
                policy.RequireClaim("hasConfirmedRegistration", "true");
            })

            // authorization policies
            .AddPolicy("UserIdAccessPolicy", policy =>
            policy.Requirements.Add(new UserIdAccessRequirement()))

            .AddPolicy("UserNameAccessPolicy", policy =>
            policy.Requirements.Add(new UserNameAccessRequirement())); ;
    }
}
