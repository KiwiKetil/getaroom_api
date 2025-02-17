using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomSchedulerAPI.Features.Models.DTOs.Token;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Services;
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.Security.Claims;

namespace RoomSchedulerAPI.Features.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {   
        // https://localhost:7089/api/v1/users?page=1&pageSize=10     
        app.MapGet("/api/v1/users", 
            async (IUserService userService,
            [AsParameters] UserQuery query,
            ILogger<Program> logger) => 
            {
                return await UserEndpointsLogic.GetAllUsersLogicAsync(userService, query, logger);
            })
        .RequireAuthorization("AdminAndPasswordUpdatedPolicy")
        .WithName("GetAllUsers");

        // https://localhost:7089/api/v1/users/887ac10b-58cc-4372-a567-0e02b2c3d493
        app.MapGet("/api/v1/users/{id}", 
            async ([FromRoute] Guid id, 
            IUserService userService, 
            ClaimsPrincipal claims,
            ILogger<Program> logger) =>
            {
                return await UserEndpointsLogic.GetUserByIdLogicAsync(id, userService, claims, logger);        
            })
        .RequireAuthorization("PasswordUpdatedPolicy")
        .WithName("GetUserById");

        // https://localhost:7089/api/v1/users/b97ac10b-58cc-4372-a567-0e02b2c3d490
        app.MapPut("/api/v1/users/{id}", 
            async ([FromRoute] Guid id,
            [FromBody] UserUpdateDTO dto,
            IUserService userService, 
            IValidator<UserUpdateDTO> validator, 
            ClaimsPrincipal claims,
            ILogger<Program> logger) =>
            {
                return await UserEndpointsLogic.UpdateUserLogicAsync(id, dto, userService, validator, claims, logger);
            })
        .RequireAuthorization("PasswordUpdatedPolicy")
        .WithName("UpdateUser");

        // https://localhost:7089/api/v1/users/6d7b1ca5-54f6-4859-a746-fc712d564128
        app.MapDelete("/api/v1/users/{id}", 
            async ([FromRoute] Guid id,
            IUserService userService, 
            ILogger<Program> logger) =>
            {
                return await UserEndpointsLogic.DeleteUserLogicAsync(id, userService, logger);
            })
        .RequireAuthorization("AdminAndPasswordUpdatedPolicy")
        .WithName("DeleteUser");

        // https://localhost:7089/api/v1/users/register
        app.MapPost("/api/v1/users/register", 
            static async ([FromBody] UserRegistrationDTO dto,
            IValidator<UserRegistrationDTO> validator, 
            IUserService userService,
            ILogger<Program> logger) =>
            {
                return await UserEndpointsLogic.RegisterUserLogicAsync(dto, validator, userService, logger);
            })
        .RequireAuthorization("AdminAndPasswordUpdatedPolicy")
        .WithName("RegisterUser");

        // https://localhost:7089/api/v1/login
        app.MapPost("/api/v1/login",
            static async ([FromBody] LoginDTO dto,
            IValidator<LoginDTO> validator,
            IUserService userService, 
            IUserAuthenticationService authService,
            ITokenGenerator tokenGenerator, 
            ILogger<Program> logger) =>
            {
                return await UserEndpointsLogic.UserLoginLogicAsync(dto, validator, userService, authService, tokenGenerator, logger);
            })
        .WithName("UserLogin");

        // https://localhost:7089/api/v1/users/change-password
        app.MapPost("/api/v1/users/update-password", 
            static async ([FromBody] UpdatePasswordDTO dto,
            IValidator<UpdatePasswordDTO> validator,
            IUserService userService,
            ITokenGenerator tokenGenerator,
            ClaimsPrincipal claims,
            ILogger<Program> logger) =>
            {
                return await UserEndpointsLogic.UpdatePasswordLogicAsync(dto, validator, userService, tokenGenerator, claims, logger);
            })
        .RequireAuthorization()
        .WithName("UpdatePassword");
    }
}
