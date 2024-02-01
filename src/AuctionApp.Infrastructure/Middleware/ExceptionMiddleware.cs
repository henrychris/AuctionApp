using System.Net;

using AuctionApp.Application.ApiResponses;

namespace AuctionApp.Infrastructure.Middleware;

/// <summary>
/// A custom exception middleware used to keep exception responses and logs consistent
/// </summary>
/// <param name="next"></param>
/// <param name="logger"></param>
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        // can catch specific exceptions here.
        catch (Exception ex)
        {
            LogException(httpContext, ex);
            await HandleExceptionAsync(httpContext);
        }
    }

    private void LogException(HttpContext httpContext, Exception ex)
    {
        var http = httpContext.GetEndpoint()?.DisplayName?.Split(" => ")[0] ?? httpContext.Request.Path.ToString();
        var httpMethod = httpContext.Request.Method;
        var type = ex.GetType().Name;
        var error = ex.Message;
        var msg =
            $"""
             Something went wrong.
             =================================
             ENDPOINT: {http}
             METHOD: {httpMethod}
             TYPE: {type}
             REASON: {error}
             ---------------------------------
             {ex.StackTrace}
             """;
        logger.LogError("{@msg}", msg);
    }

    private static Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var errors = new List<ApiError>
        {
            new()
            {
                Code = "System.InternalError",
                Description = "Something went wrong. Please reach out to an admin."
            }
        };

        var response = new ApiErrorResponse(errors, "Something went wrong. Please reach out to an admin.");
        return context.Response.WriteAsync(response.ToJsonString());
    }
}