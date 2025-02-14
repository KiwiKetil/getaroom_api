using System.Text;

namespace RoomSchedulerAPI.Features.Endpoints.Diagnostics;

public static class ApiHealthCheck
{
    public static void MapApiHealthCheckEndpoint(this WebApplication app)
    {
        app.MapGet("/api/v1/hello", () =>
        {
            string hostName = System.Net.Dns.GetHostName();
            StringBuilder sb = new();
            foreach (var adr in System.Net.Dns.GetHostEntry(hostName).AddressList)
            {
                sb.Append($"Address: {adr.AddressFamily} {adr}\n");
            }
            return $"Hello from host: {hostName}\n{sb}";
        })
        .RequireAuthorization("AdminAndPasswordUpdatedPolicy");
    }
}
