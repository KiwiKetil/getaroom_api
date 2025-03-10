using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using RoomSchedulerAPI.Core.Authorization;
using RoomSchedulerAPI.Core.DB.DBConnection;
using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using RoomSchedulerAPI.Core.DB.UnitOFWork;
using RoomSchedulerAPI.Core.DB.UnitOFWork.Interfaces;
using RoomSchedulerAPI.Core.Middleware;
using RoomSchedulerAPI.Features.AutoMapper;
using RoomSchedulerAPI.Features.Repositories;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services;
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.Globalization;
using System.Text;


namespace RoomSchedulerAPI.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost",
                builder =>
                {
                    builder
                        // .WithOrigins("http://127.0.0.1:56024", "http://localhost:56024") 
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        // MySqlConnectionFactory
        var connectionString = configuration.GetConnectionString("defaultConnection")?
                    .Replace("{ROOM_DB_USER}", Environment.GetEnvironmentVariable("ROOM_DB_USER"))
                    .Replace("{ROOM_DB_PASSWORD}", Environment.GetEnvironmentVariable("ROOM_DB_PASSWORD"));

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string is not configured.");
        }
        services.AddScoped<IDbConnectionFactory>(_ => new MySqlConnectionFactory(connectionString!));

        // Fluent Validation
        services.AddValidatorsFromAssemblyContaining<UserService>();
        services.AddFluentValidationAutoValidation(config => config.DisableDataAnnotationsValidation = true);

        ValidatorOptions.Global.LanguageManager = new LanguageManager
        {
            Culture = new CultureInfo("en")
        };

        // ExceptionHandling
        services.AddScoped<GlobalExceptionMiddleware>();

        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // ServiceLayers
        services.AddScoped<IUserService, UserService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPasswordHistoryRepository, PasswordHistoryRepository>();

        // AuthenticationService
        services.AddScoped<IPasswordVerificationService, PasswordVerificationService>();

        // TokenGenerator
        services.AddScoped<ITokenGenerator, TokenGenerator>();

        // JWT Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = configuration["Jwt:Issuer"],
                  ValidAudience = configuration["Jwt:Audience"],
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
              };
          });

        // Authorization policies
        services.AddAuthorizationBuilder()
            .AddCustomPolicies();

        services.AddScoped<IAuthorizationHandler, UserIdAccessHandler>();
        services.AddScoped<IAuthorizationHandler, UserNameAccessHandler>();

        // UnitOfWorkFactory
        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

        return services;
    }
}
