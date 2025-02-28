using FluentValidation;
using RoomSchedulerAPI.Features.Models.DTOs.ResponseDTOs;

namespace RoomSchedulerAPI.Core.Extensions;

public static class ValidationFilterExtensions
{
    public static RouteHandlerBuilder EndpointValidationFilter<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var request = context.GetArgument<TRequest>(0);

            var validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<TRequest>>();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Results.BadRequest(new ErrorResponse ( Errors: errors ));
            }

            return await next(context);
        });
    }
}
