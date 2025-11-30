using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;

namespace RequestMetrics.Workers
{
    public class SystemMetricsWorker : BackgroundService
    {
        //private readonly Process process = Process.GetCurrentProcess();
        //private TimeSpan lastCpu = TimeSpan.Zero;
        //private DateTime lastTime = DateTime.UtcNow;
        private readonly Process _process = Process.GetCurrentProcess();
        private TimeSpan _lastCpuTime;
        private DateTime _lastCheckTime;
        public SystemMetricsWorker()
        {

            // Initialize these NOW so the first loop calculation is accurate
            _lastCpuTime = _process.TotalProcessorTime;
            _lastCheckTime = DateTime.UtcNow;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //// CPU %
                //var now = DateTime.UtcNow;
                //var cpu = process.TotalProcessorTime;
                //var cpuUsage = (cpu - lastCpu).TotalMilliseconds /
                //               (now - lastTime).TotalMilliseconds /
                //               Environment.ProcessorCount * 100;

                //lastCpu = cpu;
                //lastTime = now;

                //// Memory bytes
                //process.Refresh();
                //var memoryBytes = process.WorkingSet64;

                //Log.ForContext("CPU", cpuUsage)
                //   .ForContext("MemoryBytes", memoryBytes)
                //   .ForContext("ProcessName", process.ProcessName)
                //   .Information("SYSTEM_METRIC");
                // 1. Get Current Times
                var now = DateTime.UtcNow;
                var currentCpuTime = _process.TotalProcessorTime;

                // 2. Calculate CPU %
                // Formula: (CPU Time Used / Real Time Passed) / Number of Cores * 100
                var cpuTimeUsed = (currentCpuTime - _lastCpuTime).TotalMilliseconds;
                var totalTimePassed = (now - _lastCheckTime).TotalMilliseconds;

                // Avoid division by zero if the loop runs too fast
                double cpuPercentage = 0;
                if (totalTimePassed > 0)
                {
                    cpuPercentage = cpuTimeUsed / totalTimePassed / Environment.ProcessorCount * 100;
                }

                // Update trackers for next loop
                _lastCpuTime = currentCpuTime;
                _lastCheckTime = now;

                // 3. Calculate Memory (MB)
                // Important: You must call Refresh() to get the latest memory counter
                _process.Refresh();

                // WorkingSet64 is in Bytes. Divide by 1024 twice to get MB.
                double memoryMb = _process.WorkingSet64 / 1024.0 / 1024.0;

                // 4. Log Data
                Log.ForContext("CpuPercentage", Math.Round(cpuPercentage, 2))
                   .ForContext("MemoryMB",Convert.ToInt64( Math.Round(memoryMb, 2)))
                   .ForContext("ProcessName", _process.ProcessName)
                   // Optional: Add machine name if you have multiple servers
                   .ForContext("MachineName", Environment.MachineName)
                   .Information("SYSTEM_METRIC");

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
