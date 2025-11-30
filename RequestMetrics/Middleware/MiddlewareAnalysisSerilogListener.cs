using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TestWebApp.Middleware
{
    public class MiddlewareAnalysisSerilogListener : IObserver<KeyValuePair<string, object>>
    {
        private readonly ILogger _logger;

        public MiddlewareAnalysisSerilogListener(ILogger logger)
        {
            _logger = logger;
        }

        public void OnNext(KeyValuePair<string, object> evt)
        {
            if (evt.Key == "Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareStarting")
            {
                var data = evt.Value as dynamic;
                var context = data?.HttpContext as HttpContext;
                if (context != null)
                {
                    context.Items["StartTime"] = DateTime.UtcNow;
                }
            }
            else if (evt.Key == "Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareFinished")
            {
                var data = evt.Value as dynamic;
                var context = data?.HttpContext as HttpContext;
                if (context != null && context.Items.ContainsKey("StartTime"))
                {
                    var start = (DateTime)context.Items["StartTime"];
                    var end = DateTime.UtcNow;
                    var duration = (long)(end - start).TotalMilliseconds;

                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    var route = context.Request.Path.Value ?? "unknown";

                    // Log via Serilog
                    _logger.LogInformation(
                        "api.request.duration.ms value: {Duration} | client.ip: {IP} | route: {Route} | start: {Start} | end: {End}",
                        duration, ip, route, start, end);
                }
            }
        }

        public void OnError(Exception error) { }
        public void OnCompleted() { }
    }
}
