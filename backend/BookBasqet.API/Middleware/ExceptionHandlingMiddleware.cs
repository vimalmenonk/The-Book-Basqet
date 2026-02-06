using System.Net;
using System.Text.Json;
using BookBasqet.Application.Common;

namespace BookBasqet.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleAsync(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await HandleAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception)
        {
            await HandleAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task HandleAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        var result = JsonSerializer.Serialize(ApiResponse<string>.Fail(message));
        await context.Response.WriteAsync(result);
    }
}
