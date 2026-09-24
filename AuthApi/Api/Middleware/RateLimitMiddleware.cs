using Microsoft.Extensions.Caching.Memory;

namespace AuthApi.Api.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly MemoryCache _cache = new(new MemoryCacheOptions());
        private const int Limit = 5;          // 5 requests
        private const int WindowSeconds = 60; // per 60 seconds

        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            
            if (context.Request.Path.StartsWithSegments("/api/contact/message/send", StringComparison.OrdinalIgnoreCase)
                    && context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                var key = $"rate:{ip}";
                var count = (_cache.Get<int?>(key) ?? 0) + 1;

                _cache.Set(key, count, TimeSpan.FromSeconds(WindowSeconds));

                if (count > Limit)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Too many requests. Please wait a moment and try again.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
