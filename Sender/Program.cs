using NServiceBus.Routing;

namespace Sender
{
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

            var configuration = new EndpointConfiguration();
            configuration.EndpointName("Sender");
            configuration.EnableInstallers();
            configuration.UseSerialization<JsonSerializer>();
            configuration.UsePersistence<InMemoryPersistence>();

            configuration.SendFailedMessagesTo("error");
           
            var transportConfiguration = configuration.UseTransport<AzureServiceBusTransport>();

            transportConfiguration.ConnectionString(Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString"));


            // define routing (what used to be message endpoint mapping)
            var endpointName = new EndpointName("Receiver");
           // configuration.Routing().SetMessageDistributionStrategy(new SingleInstanceRoundRobinDistributionStrategy(), t => true);
            configuration.Routing().UnicastRoutingTable.RouteToEndpoint(typeof(TestCommand), endpointName);
            configuration.Routing().EndpointInstances.AddStatic(endpointName, new EndpointInstance(endpointName, null, null));

            var endpoint = await Endpoint.Start(configuration);

            Console.WriteLine("Press ESC to exit, any other key to send a message");
            Console.WriteLine("Press any other key to send a message");
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                await endpoint.Send<TestCommand>(m => m.Data = "Testing ASB send");
                Console.WriteLine("message sent");
            }
        }
    }
}
