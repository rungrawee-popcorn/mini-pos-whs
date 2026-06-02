using System.Net;
using System.Text.Json;

namespace MiniPOSWHS.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var isApi = context.Request.Path.StartsWithSegments("/api");

            if (isApi)
            {
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = "Internal Server Error",
                    detail = ex.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            else
            {
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}