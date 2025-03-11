using FluentValidation;
using GetARoomAPI.Core.Extensions;
using GetARoomAPI.Features.Endpoints.Logic;
using GetARoomAPI.Features.Models.DTOs.UserDTOs;

namespace GetARoomAPI.Features.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        // https://localhost:7089/api/v1/users?page=1&pageSize=10     // admin only // must add role!!!
        app.MapGet("/api/v1/users", UserEndpointsLogic.GetUsersLogicAsync)
        //.RequireAuthorization("AdminRoleAndPasswordUpdatedPolicy")
        .WithName("GetUsers");
        /*
        //// https://localhost:7089/api/v1/clients?page=1&pageSize=10     // for employees and admin 
        app.MapGet("/api/v1/users", UserEndpointsLogic.GetUsersLogicAsync)
        //app.MapGet("/api/v1/clients", UserEndpointsLogic.GetClientsLogicAsync)
        ////.RequireAuthorization("AdminRoleAndPasswordUpdatedPolicy")
        //.WithName("GetClients");

        // https://localhost:7089/api/v1/users/887ac10b-58cc-4372-a567-0e02b2c3d493 // admin only
        app.MapGet("/api/v1/users/{id}", UserEndpointsLogic.GetUserByIdLogicAsync)
        .RequireAuthorization("UserOrAdminRoleAndPasswordUpdatedPolicy")
        .WithName("GetUserById");

        // https://localhost:7089/api/v1/users/b97ac10b-58cc-4372-a567-0e02b2c3d490
        app.MapPut("/api/v1/users/{id}", UserEndpointsLogic.UpdateUserLogicAsync)
        .RequireAuthorization("UserOrAdminRoleAndPasswordUpdatedPolicy") // include employees all
        .EndpointValidationFilter<UserUpdateDTO>()
        .WithName("UpdateUser");

        // https://localhost:7089/api/v1/users/6d7b1ca5-54f6-4859-a746-fc712d564128  // admin only
        app.MapDelete("/api/v1/users/{id}", UserEndpointsLogic.DeleteUserLogicAsync)
        .RequireAuthorization("AdminRoleAndPasswordUpdatedPolicy")
        .WithName("DeleteUser");
        */
        // https://localhost:7089/api/v1/employees/register  // register employee, admin only
        app.MapPost("/api/v1/employees/register", UserEndpointsLogic.RegisterEmployeeLogicAsync)
        //.RequireAuthorization("AdminRoleAndPasswordUpdatedPolicy")
        .EndpointValidationFilter<UserRegistrationDTO>()
        .WithName("RegisterEmployee");

        // https://localhost:7089/api/v1/clients/register   // this will register client, employee only
        app.MapPost("/api/v1/clients/register", UserEndpointsLogic.RegisterClientLogicAsync)
        // .RequireAuthorization("AdminRoleAndPasswordUpdatedPolicy")
        .EndpointValidationFilter<UserRegistrationDTO>()
        .WithName("RegisterClient");
        /*
        // https://localhost:7089/api/v1/login
        app.MapPost("/api/v1/login", UserEndpointsLogic.UserLoginLogicAsync)
        .EndpointValidationFilter<LoginDTO>()
        .WithName("UserLogin");

        // https://localhost:7089/api/v1/users/update-password
        app.MapPost("/api/v1/users/update-password", UserEndpointsLogic.UpdatePasswordLogicAsync)
        .RequireAuthorization()
        .EndpointValidationFilter<UpdatePasswordDTO>()
        .WithName("UpdatePassword");
        */
    }
}
