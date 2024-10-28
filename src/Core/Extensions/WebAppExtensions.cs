
using RoomSchedulerAPI.Core.DBConnection;
using RoomSchedulerAPI.Core.Exceptions;
using RoomSchedulerAPI.Core.Middleware;
using RoomSchedulerAPI.Features.Repositories;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services;
using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.Core.Extensions;

public static class WebAppExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)    
    {
        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // MySqlConnectionFactory
        var connectionString = configuration.GetConnectionString("defaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'defaultConnection' is not configured.");
        }
        services.AddScoped(provider =>
            new MySqlConnectionFactory(connectionString));

        // ExceptionHandling
        services.AddScoped<GlobalExceptionMiddleware>();
        services.AddSingleton<ExceptionHandler>();

        // ServiceLayers
        services.AddScoped<IUserService, UserService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}