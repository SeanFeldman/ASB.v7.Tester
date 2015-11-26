namespace Sender
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using NServiceBus;
    using NServiceBus.Config;
    using NServiceBus.Config.ConfigurationSource;
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
            configuration.EndpointName("Sender");
            configuration.EnableInstallers();
            configuration.UseSerialization<JsonSerializer>();
            configuration.UsePersistence<InMemoryPersistence>();

            configuration.SendFailedMessagesTo("error");
            configuration.UseTransport<AzureServiceBusTransport>()
                .UseDefaultTopology()
                .ConnectionString(Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString"));

            // define routing (what used to be message endpoint mapping)
            var endpointName = new EndpointName("Receiver");
            configuration.Routing().UnicastRoutingTable.AddStatic(typeof(TestCommand), endpointName);
            configuration.Routing().EndpointInstances.AddStatic(endpointName, new EndpointInstanceName(endpointName, null, null));

            var endpoint = await Endpoint.Start(configuration);
            var context = endpoint.CreateBusContext();

            Console.WriteLine("Press ESC to exit, any other key to send a message");
            Console.WriteLine("Press any other key to send a message");
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                await context.Send<TestCommand>(m => m.Data = "Testing ASB send");
                Console.WriteLine("message sent");
            }
        }
    }

//    public class MapMessages : IProvideConfiguration<UnicastBusConfig>
//    {
//        public UnicastBusConfig GetConfiguration()
//        {
//            return new UnicastBusConfig
//            {
//                MessageEndpointMappings = new MessageEndpointMappingCollection
//                {
//                    new MessageEndpointMapping
//                    {
//                        Endpoint = "Receiver",
//                        AssemblyName = "Messages"
//                    }
//                }
//            };
//        }
//    }
}
