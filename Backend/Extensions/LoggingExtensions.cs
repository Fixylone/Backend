using Serilog;

namespace Backend.Extensions
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Adds the service logging.
        /// </summary>
        /// <param name="hostBuilder"><see cref="IHostBuilder"/>.</param>
        public static void AddServiceLogging(this IHostBuilder hostBuilder)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configBuilder)
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithProcessName()
                .Enrich.WithAssemblyName()
                .Enrich.WithAssemblyVersion();

            Serilog.Log.Logger = loggerConfiguration.CreateLogger();
        }

        /// <summary>
        /// Uses the service loggings.
        /// </summary>
        /// <param name="hostBuilder"><see cref="IHostBuilder"/>.</param>
        public static void UseServiceLogging(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog();
        }
    }
}
