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
        .RequireAuthorization("EmployeeOrAdminWithUpdatedPasswordPolicy")
        .WithName("GetUsers");

        // https://localhost:7089/api/v1/users/887ac10b-58cc-4372-a567-0e02b2c3d493 
        app.MapGet("/api/v1/users/{id}", UserEndpointsLogic.GetUserByIdLogicAsync)
        .RequireAuthorization("AdminWithUpdatedPasswordPolicy")
        .WithName("GetUserById");

        // https://localhost:7089/api/v1/users/b97ac10b-58cc-4372-a567-0e02b2c3d490 
        app.MapPut("/api/v1/users/{id}", UserEndpointsLogic.UpdateUserLogicAsync)
        .RequireAuthorization() 
        .EndpointValidationFilter<UserUpdateDTO>()
        .WithName("UpdateUser");       

        // https://localhost:7089/api/v1/users/6d7b1ca5-54f6-4859-a746-fc712d564128 
        app.MapDelete("/api/v1/users/{id}", UserEndpointsLogic.DeleteUserLogicAsync)
        .RequireAuthorization("AdminWithUpdatedPasswordPolicy")
        .WithName("DeleteUser");

        // https://localhost:7089/api/v1/employees/register  // register employee, admin only // samel emd den under, kun 1 register? hmm.. when register employee - employee gets email to set own passsword!
        app.MapPost("/api/v1/employees/register", UserEndpointsLogic.RegisterEmployeeLogicAsync) // husk if user already exist må kun throw ex alreadyexist dersom rollen er den samme.. 
        //.RequireAuthorization("AdminWithUpdatedPasswordPolicy")
        .EndpointValidationFilter<UserRegistrationDTO>()
        .WithName("RegisterEmployee");

        // https://localhost:7089/api/v1/clients/register   // this will register client. Employee can register at desk and client can at home, admin can. If register at desk, client gets
                                                            // email - confirm and set password? if they want acceess to bookings etc. If register at home - client sets password and done.
                                                            
        app.MapPost("/api/v1/clients/register", UserEndpointsLogic.RegisterClientLogicAsync)
        // .RequireAuthorization("AdminWithUpdatedPasswordPolicy")
        .EndpointValidationFilter<UserRegistrationDTO>()
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

    }
}
