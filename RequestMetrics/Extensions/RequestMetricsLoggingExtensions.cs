using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;

namespace RequestMetrics.Extensions
{
    public static class RequestMetricsLoggingExtensions
    {
        public static IHostBuilder UseMySerilog(this IHostBuilder hostBuilder)
        {
            
            hostBuilder.ConfigureAppConfiguration((ctx, config) =>
            {
                // Add your JSON file here
                config.AddJsonFile("SerilogSetting.json", optional: true, reloadOnChange: true);
            });

            hostBuilder.UseSerilog((ctx, lc) =>
            {
                // Enable Serilog internal errors
                SelfLog.Enable(msg => Console.WriteLine(msg));

                lc.ReadFrom.Configuration(ctx.Configuration);
            });

            return hostBuilder;
        }
    }
}
