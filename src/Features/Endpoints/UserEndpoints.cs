using FluentValidation;
using RoomSchedulerAPI.Core.Extensions;
using RoomSchedulerAPI.Features.Endpoints.Logic;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;

namespace RoomSchedulerAPI.Features.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        // https://localhost:7089/api/v1/users?page=1&pageSize=10     
        app.MapGet("/api/v1/users", UserEndpointsLogic.GetUsersLogicAsync)
        .RequireAuthorization("AdminAndPasswordUpdatedPolicy")
        .WithName("GetUsers");

        // https://localhost:7089/api/v1/users/887ac10b-58cc-4372-a567-0e02b2c3d493
        app.MapGet("/api/v1/users/{id}", UserEndpointsLogic.GetUserByIdLogicAsync)
        .RequireAuthorization("PasswordUpdatedPolicy")
        .WithName("GetUserById");

        // https://localhost:7089/api/v1/users/b97ac10b-58cc-4372-a567-0e02b2c3d490
        app.MapPut("/api/v1/users/{id}", UserEndpointsLogic.UpdateUserLogicAsync)
        .RequireAuthorization("PasswordUpdatedPolicy")
        .WithName("UpdateUser")
        .EndpointValidationFilter<UserUpdateDTO>();

        // https://localhost:7089/api/v1/users/6d7b1ca5-54f6-4859-a746-fc712d564128
        app.MapDelete("/api/v1/users/{id}", UserEndpointsLogic.DeleteUserLogicAsync)
        .RequireAuthorization("AdminAndPasswordUpdatedPolicy")
        .WithName("DeleteUser");

        // https://localhost:7089/api/v1/users/register
        app.MapPost("/api/v1/users/register", UserEndpointsLogic.RegisterUserLogicAsync)
        .RequireAuthorization("AdminAndPasswordUpdatedPolicy")
        .WithName("RegisterUser")
        .EndpointValidationFilter<UserRegistrationDTO>();

        // https://localhost:7089/api/v1/login
        app.MapPost("/api/v1/login", UserEndpointsLogic.UserLoginLogicAsync)
        .WithName("UserLogin")
        .EndpointValidationFilter<LoginDTO>();

        // https://localhost:7089/api/v1/users/update-password
        app.MapPost("/api/v1/users/update-password", UserEndpointsLogic.UpdatePasswordLogicAsync)
        .RequireAuthorization()
        .WithName("UpdatePassword")
        .EndpointValidationFilter<UpdatePasswordDTO>();
    }
}
