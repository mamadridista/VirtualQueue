using System.Net;
using System.Text.Json;
using VirtualQueue.Domain.Exceptions;

namespace VirtualQueue.Api.Middleware;

/// <summary>
/// Catches unhandled exceptions at the edge of the pipeline so controllers
/// don't need repetitive try/catch blocks. Maps known domain exceptions to
/// meaningful HTTP status codes and returns a consistent JSON error shape;
/// anything unexpected becomes a 500 with no internal details leaked.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain rule violation");
            await WriteErrorResponseAsync(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponseAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new
        {
            status = (int)statusCode,
            error = message
        });

        await context.Response.WriteAsync(payload);
    }
}
