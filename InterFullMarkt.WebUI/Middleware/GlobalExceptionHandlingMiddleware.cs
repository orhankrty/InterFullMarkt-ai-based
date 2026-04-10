namespace InterFullMarkt.WebUI.Middleware;

using System.Text.Json;

/// <summary>
/// Global Exception Handling Middleware.
/// Tüm unhandled exception'ları yakalar ve uygun HTTP response döndürür.
/// </summary>
public sealed class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Exception'u HTTP response'a dönüştürür.
    /// </summary>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            message = exception.Message,
            type = exception.GetType().Name,
            timestamp = DateTime.UtcNow
        };

        // Exception type'ına göre HTTP status code belirle
        var (statusCode, title) = exception switch
        {
            ArgumentNullException => (StatusCodes.Status400BadRequest, "Invalid Request"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Argument"),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid Operation"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsJsonAsync(new
        {
            title,
            statusCode,
            detail = response.message,
            instance = context.Request.Path,
            timestamp = response.timestamp
        }, options);
    }
}
