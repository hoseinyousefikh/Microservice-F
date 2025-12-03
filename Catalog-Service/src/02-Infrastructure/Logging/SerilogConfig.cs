using Serilog;
using Serilog.Events;

namespace Catalog_Service.src._02_Infrastructure.Logging
{
    using System;
    using Serilog;
    using Serilog.Events;
    using Microsoft.Extensions.Configuration;

    namespace Catalog_Service.src._02_Infrastructure.Logging
    {
        public static class SerilogConfig
        {
            public static void Configure(IConfiguration configuration)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("MachineName", Environment.MachineName)
                    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development")
                    .Enrich.WithProperty("ProcessId", Environment.ProcessId)
                    .Enrich.WithProperty("ThreadId", System.Threading.Thread.CurrentThread.ManagedThreadId)
                    .Enrich.With<CustomLogEnricher>()
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File(
                        path: "logs/catalog-service-.txt",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}]")
                    .CreateLogger();
            }
        }
    }
}
