using Microsoft.AspNetCore.Mvc;
using static RoomSchedulerAPI.Core.Exceptions.CustomExceptions;

namespace RoomSchedulerAPI.Core.Exceptions;

public class ExceptionHandler(ILogger<ExceptionHandler> logger)
{
    private readonly ILogger<ExceptionHandler> _logger = logger;

    // called from GlobalExceptionMiddlerware.cs located in Middleware folder
    public async Task HandleException(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "An error occurred - {@Machine} {@TraceId}",
            Environment.MachineName,
            System.Diagnostics.Activity.Current?.Id);

        var (statusCode, title) = ex switch
        {
            UserAlreadyExistsException => (StatusCodes.Status409Conflict, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Extensions = { ["traceId"] = System.Diagnostics.Activity.Current?.Id }
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}

