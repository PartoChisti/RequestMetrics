using Microsoft.AspNetCore.Builder;
using RequestMetrics.Middleware;

namespace RequestMetrics.Extensions
{
    public static class RequestMetricsMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestMetrics(this IApplicationBuilder app)
        {
            app.UseMiddleware<ApiMetricsMiddleware>();
            return app;
        }
    }
}
