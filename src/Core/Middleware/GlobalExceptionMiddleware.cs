using RoomSchedulerAPI.Core.Exceptions;

namespace RoomSchedulerAPI.Core.Middleware;

public class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ExceptionHandler _exceptionHandler;

    public GlobalExceptionMiddleware(ExceptionHandler exceptionHandler)
    {
        _exceptionHandler = exceptionHandler;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await _exceptionHandler.HandleException(context, ex);
        }
    }
}