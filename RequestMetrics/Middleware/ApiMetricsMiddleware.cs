using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;

namespace RequestMetrics.Middleware
{
    public class ApiMetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private bool IsApiRequest(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            //if (!path.StartsWith("/api"))
                //return false;

            string[] ignoreExtensions = {
            ".js",".css",".map",".png",".jpg",".jpeg",".gif",
            ".svg",".ico",".json",".txt",".html",".wasm"
        };

            return !ignoreExtensions.Any(ext => path.EndsWith(ext));
        }
        public ApiMetricsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var route = context.Request.Path;
            if (!IsApiRequest(route.Value?.ToLower()))
            {
                await _next(context);
                return;
            }
            var sw = Stopwatch.StartNew();
            var method = context.Request.Method;
            var port = context.Connection.LocalPort;
            var routeString = route.ToString();
            Log.ForContext("ApiRoute", routeString)
              .ForContext("HttpMethod", method)
              .ForContext("LocalPort", port)
              .Information("API_METRIC");


            await _next(context);

            sw.Stop();
            Log.ForContext("ApiRoute", routeString)
               .ForContext("HttpMethod", method)
               .ForContext("LocalPort", port)
               .ForContext("StatusCode", context.Response.StatusCode)
               .ForContext("DurationMs", sw.ElapsedMilliseconds)
               .Information("API_METRIC");
               //.Information("API_CALL_METRIC");
        }
    }
}
