using Microsoft.Extensions.Options;
using RoomSchedulerAPI.Core.Dapper;

namespace RoomSchedulerAPI.Core.Extensions;

public static class WebAppExtensions
{
    //public static void RegisterMappers(this WebApplicationBuilder builder)
    //{
    //    var assembly = typeof(UserMapper).Assembly;

    //    var mapperTypes = assembly.GetTypes()
    //        .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
    //        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<,>)))
    //        .ToList();

    //    foreach (var mapperType in mapperTypes)
    //    {
    //        var interfaceType = mapperType.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(IMapper<,>));
    //        builder.Services.AddScoped(interfaceType, mapperType);
    //    }
    //}

    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)    
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddSingleton(new MySqlConnectionFactory(configuration.GetConnectionString("defaultConnection")!));

        //services.AddSingleton(serviceProvider =>
        //{            
        //    return new MySqlConnectionFactory(configuration.GetConnectionString("defaultConnection")!);
        //});

        return services;
    }
}

