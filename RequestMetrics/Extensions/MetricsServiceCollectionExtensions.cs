using Microsoft.Extensions.DependencyInjection;
using RequestMetrics.Workers;

namespace RequestMetrics.Extensions
{
    public static class MetricsServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemMetrics(this IServiceCollection services)
        {
            services.AddHostedService<SystemMetricsWorker>();
            return services;
        }
    }
}
