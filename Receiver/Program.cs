using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

class Program
{
    static void Main()
    {
        MainAsync().GetAwaiter().GetResult();
    }

    private static async Task MainAsync()
    {
        var factory = LogManager.Use<DefaultFactory>();
        factory.Level(LogLevel.Debug);

        var configuration = new BusConfiguration();
        configuration.EndpointName("Receiver");
        configuration.EnableInstallers();
        configuration.UseSerialization<JsonSerializer>();
        configuration.UsePersistence<InMemoryPersistence>();
        configuration.SendFailedMessagesTo("error");
        configuration.UseTransport<AzureServiceBusTransport>()
            .UseDefaultTopology()
            .ConnectionString(Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString"));

        var endpoint = await Endpoint.Start(configuration);

        Console.WriteLine("Press ESC to exit");
        Console.WriteLine("listening to messages");
        while (Console.ReadKey().Key != ConsoleKey.Escape)
        {
        }

        await endpoint.Stop();
    }

    class Handler : IHandleMessages<TestCommand>
    {
        public Task Handle(TestCommand message, IMessageHandlerContext context)
        {
            Console.WriteLine("Received: " + message.Data);

            return Task.FromResult(0);
        }
    }
}