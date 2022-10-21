using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starging Demo...");

            using var channel = new InMemoryChannel();

            try
            {
                IServiceCollection services = new ServiceCollection();
                services.Configure<TelemetryConfiguration>(config => config.TelemetryChannel = channel);
                services.AddLogging(builder =>
                {
                    const string ConnectionString = "";

                    builder.AddApplicationInsights(
                        configureTelemetryConfiguration: (config) =>
                        {
                            config.ConnectionString = ConnectionString;
                        },
                        configureApplicationInsightsLoggerOptions: (options) => { }
                    );
                });

                IServiceProvider serviceProvider = services.BuildServiceProvider();
                ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogInformation($"Hello from Logger");
                logger.LogWarning($"It is now {DateTime.Now}");

                string scopeName = $"Demo Scope {Guid.NewGuid()}";
                Console.WriteLine($"Entering scope: {scopeName}");
                using (logger.BeginScope(scopeName)) {
                    logger.LogInformation("This message is in scope");
                    logger.LogInformation("This message is also in scope");
                } 
                Console.WriteLine($"Exiting scope: {scopeName}");

                logger.LogError("Oups! We got an error!");
            }
            finally
            {
                channel.Flush();

                await Task.Delay(TimeSpan.FromMilliseconds(1000));
            }

            Console.WriteLine("Done!");
        }
    }
}