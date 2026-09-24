namespace AuthApi.Api.Middleware;
public class HttpRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HttpRequestLoggingMiddleware> _logger;
    public HttpRequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<HttpRequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "<- {StatusCode} {Method} {Path} ({ElapsedMs}ms)",
                context.Response.StatusCode,
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
