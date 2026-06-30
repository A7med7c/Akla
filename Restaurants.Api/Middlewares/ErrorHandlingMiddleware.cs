
using Restaurants.Domain.Exceptions;
using System.Text.Json;

namespace Restaurants.Api.Middlewares;

public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (NotFoundException notFound)
        {
            logger.LogWarning(notFound, "Requested resource was not found.");

            await WriteErrorResponse(context, StatusCodes.Status404NotFound, notFound.Message);
        }
        catch (ForbiddenException)
        {
            logger.LogWarning("Forbidden request to {Method} {Path}.", context.Request.Method, context.Request.Path);

            await WriteErrorResponse(context, StatusCodes.Status403Forbidden, "Access forbidden");
        }
        catch (ExternalStorageException storageException)
        {
            logger.LogError(
                storageException,
                "External storage failed while processing {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);

            await WriteErrorResponse(
                context,
                StatusCodes.Status503ServiceUnavailable,
                storageException.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unhandled exception while processing {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);

            await WriteErrorResponse(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred. Please contact support.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = JsonSerializer.Serialize(new
        {
            statusCode,
            message
        });

        await context.Response.WriteAsync(response);
    }
}
