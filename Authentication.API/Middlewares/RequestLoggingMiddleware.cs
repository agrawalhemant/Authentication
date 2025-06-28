namespace Authentication.API.Middlewares;

public class RequestLoggingMiddleware : IMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var request = context.Request;
        _logger.LogInformation("HTTP {Method} {Path}", request.Method, request.Path);

        var start = DateTime.UtcNow;
        await next(context);
        var duration = DateTime.UtcNow - start;

        _logger.LogInformation("Completed with {StatusCode} status in {Duration} ms", context.Response.StatusCode, duration.TotalMilliseconds);
    }
}