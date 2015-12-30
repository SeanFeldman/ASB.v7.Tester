using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.AzureServiceBus;
using NServiceBus.AzureServiceBus.Addressing;
using NServiceBus.Configuration.AdvanceExtensibility;
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

        // minimal requirement
        var transportConfiguration = configuration.UseTransport<AzureServiceBusTransport>();
                         
        transportConfiguration.ConnectionString(Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString"));// same as AddNamespace?
        //transportConfiguration.ConnectionStringName("???") do we support this?

        //transportConfiguration.AddAddressTranslationException() // what's this?
        //transportConfiguration.AddAddressTranslationRule() // and this?
        //transportConfiguration.Transactions(TransportTransactionMode.TransactionScope); //why doesn't this blow up? not supported
        //transportConfiguration.SubscriptionAuthorizer(); // what's this?

        //transportConfiguration.Batching().??? // remove, no extensions
        // we should drop the toplevels Connectivity(), Serialization(), Addressing(), Resources()
        //transportConfiguration.Connectivity().NumberOfClientsPerEntity(1); // turn into property
        //transportConfiguration.Connectivity().SendViaReceiveQueue(false); // turn into property
        //transportConfiguration.Connectivity().MessageReceivers().AutoRenewTimeout(TimeSpan.MaxValue); // turn into properties, MessageReceiverSettings.AutoRenewTimeout = TimeSpan.MaxValue
        //transportConfiguration.Connectivity().MessageSenders().BackOffTimeOnThrottle(TimeSpan.MaxValue); // turn into properties, MessageSenderSettings.BackOffTimeOnThrottle = TimeSpan.MaxValue
        //transportConfiguration.Connectivity().MessagingFactories().BatchFlushInterval(TimeSpan.MaxValue); // turn into properties, MessagingFactorySettings.BatchFlushInterval = TimeSpan.MaxValue

        //transportConfiguration.Serialization().BrokeredMessageBodyType(SupportedBrokeredMessageBodyTypes.Stream);
        //transportConfiguration.UseDefaultTopology() // remove, not needed
        var topologySettings = transportConfiguration.UseTopology<StandardTopology>();
        //topologySettings.Connectivity() make sure these do not popup in intellisense
        //topologySettings.Addressing().Composition().UseStrategy<FlatCompositionStrategy>() turn into topologySettings.Addressing.UseCompositionStrategy<FlatCompositionStrategy>()
        //topologySettings.Addressing().Individualization().UseStrategy<>() same as composition
        //topologySettings.Addressing().NamespacePartitioning().UseStrategy<>()
        //topologySettings.Addressing().NamespacePartitioning().AddNamespace() 
        //topologySettings.Addressing().Sanitization().UseStrategy<>()
        //topologySettings.Addressing().Validation().UseStrategy<>()
        //topologySettings.Addressing().Validation().UseQueuePathMaximumLength() //shouldn't these be on resources instead?
        //topologySettings.Addressing().Validation().UseTopicPathMaximumLength() //shouldn't these be on resources instead?
        //topologySettings.Addressing().Validation().UseSubscriptionPathMaximumLength() //shouldn't these be on resources instead?

        //topologySettings.Resources().Queues().AutoDeleteOnIdle() // change to properties topologySettings.Resources.Queues.AutoDeleteOnIdle =  true
        //topologySettings.Resources().Queues().DescriptionFactory(func) //Change to SetDescriptionFactory
        
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