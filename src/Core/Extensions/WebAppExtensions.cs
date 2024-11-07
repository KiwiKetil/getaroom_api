
using RoomSchedulerAPI.Core.DB.DBConnection;
using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using RoomSchedulerAPI.Core.Middleware;
using RoomSchedulerAPI.Features.AutoMapper;
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
            throw new InvalidOperationException("Connection string is not configured.");
        }

        connectionString = connectionString?
        .Replace("{ROOM_DB_USER}", Environment.GetEnvironmentVariable("ROOM_DB_USER"))
        .Replace("{ROOM_DB_PASSWORD}", Environment.GetEnvironmentVariable("ROOM_DB_PASSWORD"));

        services.AddScoped<IDbConnectionFactory>(_ => new MySqlConnectionFactory(connectionString!));

        // ExceptionHandling
        services.AddScoped<GlobalExceptionMiddleware>();

        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // ServiceLayers
        services.AddScoped<IUserService, UserService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}