using DotnetKafkaMirror.Configuration;
using DotnetKafkaMirror.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DotnetKafkaMirror
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            var serviceCollection = new ServiceCollection()
                .AddSingleton<IMirrorTopicHandler, MirrorTopicHandler>()
                .AddSingleton<IMirrorProcessor, MirrorProcessor>()
                .AddSingleton<IEnvironmentConfigProvider, EnvironmentConfigProvider>()
                .AddSingleton<IKafkaConsumer, KafkaConsumer>()
                .AddSingleton<IKafkaProducer, KafkaProducer>()
                .BuildServiceProvider();

            serviceCollection.GetService<IMirrorProcessor>().Run();
        }

    }
}
