
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.Data;

namespace RoomSchedulerAPI.Features.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/api/v1/users", async (IUserService userService) =>
        {

        });

        app.MapGet("/api/test-exception", () =>
        {
            throw new InvalidOperationException("This is a test exception.");
        });

    }
}
