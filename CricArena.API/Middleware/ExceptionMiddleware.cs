using CricArena.API.Models;
using CricArena.Business.Exceptions;
using System.Text.Json;

namespace CricArena.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    //This is a mandatory method which needs to be implemented in the middleware class. This method will be called for every HTTP request.
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception occurred.");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType =
            "application/json";

        switch (exception)
        {
            case DuplicateEmailException duplicateEmail:

                context.Response.StatusCode = 409;

                break;

            case PlayerNotFoundException:

            case ClubNotFoundException:

            case MembershipNotFoundException:

                context.Response.StatusCode = 404;

                break;

            case ArgumentException:

                context.Response.StatusCode = 400;

                break;

            case UnauthorizedAccessException:

                context.Response.StatusCode = 403;

                break;

            case InvalidOperationException:
                context.Response.StatusCode = 400;
                break;

            default:

                context.Response.StatusCode = 500;

                break;
        }

        var response = new ApiErrorResponse
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message,
            TraceId = context.TraceIdentifier
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
