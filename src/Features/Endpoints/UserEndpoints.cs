using FluentValidation;
using GetARoomAPI.Core.Extensions;
using GetARoomAPI.Features.Endpoints.Logic;
using GetARoomAPI.Features.Models.DTOs.UserDTOs;

namespace GetARoomAPI.Features.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        // https://localhost:7089/api/v1/users?page=1&pageSize=10     
        app.MapGet("/api/v1/users", UserEndpointsLogic.GetUsersLogicAsync)
        .RequireAuthorization("EmployeeOrAdminWithConfirmedRegistrationPolicy")
        .WithName("GetUsers");

        // https://localhost:7089/api/v1/users/887ac10b-58cc-4372-a567-0e02b2c3d493 
        app.MapGet("/api/v1/users/{id}", UserEndpointsLogic.GetUserByIdLogicAsync)
        .RequireAuthorization("AdminWithConfirmedRegistrationPolicy")
        .WithName("GetUserById");

        // https://localhost:7089/api/v1/users/b97ac10b-58cc-4372-a567-0e02b2c3d490 
        app.MapPut("/api/v1/users/{id}", UserEndpointsLogic.UpdateUserLogicAsync)
        .RequireAuthorization() 
        .EndpointValidationFilter<UserUpdateDTO>()
        .WithName("UpdateUser");       

        // https://localhost:7089/api/v1/users/6d7b1ca5-54f6-4859-a746-fc712d564128 
        app.MapDelete("/api/v1/users/{id}", UserEndpointsLogic.DeleteUserLogicAsync)
        .RequireAuthorization("AdminWithConfirmedRegistrationPolicy")
        .WithName("DeleteUser");

        // https://localhost:7089/api/v1/employees/register 
        app.MapPost("/api/v1/employees/register", UserEndpointsLogic.RegisterEmployeeLogicAsync)
        .RequireAuthorization("AdminWithConfirmedRegistrationPolicy")
        .EndpointValidationFilter<EmployeeRegistrationDTO>() // EmployeeRegistrationDTO? ikke sette passord her, employee gjøre vi email når confirmemail..?
        .WithName("RegisterEmployee");

        // https://localhost:7089/api/v1/clients/register                                                               
        app.MapPost("/api/v1/clients/register", UserEndpointsLogic.RegisterClientLogicAsync) // For clients registering themselves. Set password at this time. Receive confirmationemail to verify.
        .EndpointValidationFilter<ClientRegistrationDTO>() // ClienttregistrationDTO?
        .WithName("RegisterClient");

        // https://localhost:7089/api/v1/clients/register // only for employees registering clients. No password set must be set during email confirmation.
        app.MapPost("/api/v1/clients/employee-register", UserEndpointsLogic.RegisterClientLogicAsync)
        .EndpointValidationFilter<ClientRegistrationDTO>() // EmployeeClientregistrationDTO?
        .WithName("RegisterClient");

        // https://localhost:7089/api/v1/login
        app.MapPost("/api/v1/login", UserEndpointsLogic.UserLoginLogicAsync)
        .EndpointValidationFilter<LoginDTO>()
        .WithName("UserLogin");

        // https://localhost:7089/api/v1/users/update-password
        app.MapPost("/api/v1/users/update-password", UserEndpointsLogic.UpdatePasswordLogicAsync)
        .RequireAuthorization()
        .EndpointValidationFilter<UpdatePasswordDTO>()
        .WithName("UpdatePassword");

        // https://localhost:7089/api/v1/users/confirm-registration
        app.MapGet("/api/v1/users/confirm-registration", UserEndpointsLogic.ConfirmRegistrationAsync)
        .WithName("ConfirmRegistration");
    }
}
