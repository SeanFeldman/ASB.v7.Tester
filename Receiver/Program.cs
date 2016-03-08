using System;
using System.Threading.Tasks;
using Messages;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NServiceBus;
using NServiceBus.AzureServiceBus;
// ReSharper disable RedundantUsingDirective
using NServiceBus.AzureServiceBus.Addressing;
using NServiceBus.Configuration.AdvanceExtensibility;
// ReSharper restore RedundantUsingDirective
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
        configuration.EndpointName("Receiver");
        configuration.EnableInstallers();
        configuration.UseSerialization<JsonSerializer>();
        configuration.UsePersistence<InMemoryPersistence>();
        configuration.SendFailedMessagesTo("error");

        // minimal requirement
        var transportConfiguration = configuration.UseTransport<AzureServiceBusTransport>();

        transportConfiguration.ConnectionString(Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString"));// same as AddNamespace?
                                                                                                                        //        transportConfiguration.ConnectionStringName("???"); // do we support this?

        //        transportConfiguration.AddAddressTranslationException(); // what's this?
        //        transportConfiguration.AddAddressTranslationRule(); // and this?
        //        transportConfiguration.Transactions(TransportTransactionMode.TransactionScope); //why doesn't this blow up? not supported
        //        transportConfiguration.SubscriptionAuthorizer(context => true); // what's this?

        //transportConfiguration.Batching().??? // remove, no extensions => https://github.com/Particular/NServiceBus.AzureServiceBus/issues/92#issuecomment-193514070

        // we should drop the toplevels Connectivity(), Serialization(), Addressing(), Resources()

        //        transportConfiguration.Connectivity().NumberOfClientsPerEntity(1); // turn into property
        //        transportConfiguration.Connectivity().SendViaReceiveQueue(false); // turn into property
        //        transportConfiguration.Connectivity().ConnectivityMode(ConnectivityMode.Https);
        //
        //        transportConfiguration.Connectivity().MessageSenders().MaximuMessageSizeInKilobytes(200);
        //        transportConfiguration.Connectivity().MessageSenders().MessageSizePaddingPercentage(5);
        //        transportConfiguration.Connectivity().MessageSenders().BackOffTimeOnThrottle(TimeSpan.FromMilliseconds(500));
        //        transportConfiguration.Connectivity().MessageSenders().OversizedBrokeredMessageHandler(new NoOpOversizedBrokeredMessagesHandler());
        //        transportConfiguration.Connectivity().MessageSenders().RetryAttemptsOnThrottle(10);
        //        transportConfiguration.Connectivity().MessageSenders().RetryPolicy(RetryPolicy.Default);
        //
        //        transportConfiguration.Connectivity().MessageReceivers().AutoRenewTimeout(TimeSpan.MaxValue); // turn into properties, MessageReceiverSettings.AutoRenewTimeout = TimeSpan.MaxValue
        //        transportConfiguration.Connectivity().MessageReceivers().PrefetchCount(50);
        //        transportConfiguration.Connectivity().MessageReceivers().ReceiveMode(ReceiveMode.ReceiveAndDelete);
        //        transportConfiguration.Connectivity().MessageReceivers().RetryPolicy(RetryPolicy.Default);
        //
        //        transportConfiguration.Connectivity().MessagingFactories().BatchFlushInterval(TimeSpan.MaxValue); // turn into properties, MessagingFactorySettings.BatchFlushInterval = TimeSpan.MaxValue
        //        transportConfiguration.Connectivity().MessagingFactories().BatchFlushInterval(TimeSpan.FromMilliseconds(300));
        //        transportConfiguration.Connectivity().MessagingFactories().MessagingFactorySettingsFactory(endpointName => new MessagingFactorySettings());
        //        transportConfiguration.Connectivity().MessagingFactories().NumberOfMessagingFactoriesPerNamespace(10);
        //        transportConfiguration.Connectivity().MessagingFactories().PrefetchCount(100); // <== not clear what's different about this and MessageReceivers().PrefetchCount()
        //        transportConfiguration.Connectivity().MessagingFactories().RetryPolicy(RetryPolicy.Default);

        //        transportConfiguration.Serialization().BrokeredMessageBodyType(SupportedBrokeredMessageBodyTypes.Stream);

        //        transportConfiguration.UseDefaultTopology(); // remove, not needed

        // ReSharper disable once UnusedVariable
        var topologySettings = transportConfiguration.UseTopology<StandardTopology>();
        //topologySettings.Connectivity() make sure these do not popup in intellisense

        //        topologySettings.Addressing().Composition().UseStrategy<FlatCompositionStrategy>(); //turn into topologySettings.Addressing.UseCompositionStrategy<FlatCompositionStrategy>()
        //        topologySettings.Addressing().Individualization().UseStrategy<CoreIndividualizationStrategy>(); // same as composition
        //        topologySettings.Addressing().NamespacePartitioning().UseStrategy<ShardedNamespacePartitioningStrategy>();
        //        topologySettings.Addressing().NamespacePartitioning().AddNamespace("fallback", "sb://connection.string");

        //        topologySettings.Addressing().Sanitization().UseStrategy<ThrowOnFailingSanitizationStrategy>();

        //        topologySettings.Addressing().Validation().UseStrategy<EntityNameValidationRules>();
        //        topologySettings.Addressing().Validation().UseQueuePathMaximumLength(200); //shouldn't these be on resources instead?
        //        topologySettings.Addressing().Validation().UseTopicPathMaximumLength(200); //shouldn't these be on resources instead?
        //        topologySettings.Addressing().Validation().UseSubscriptionPathMaximumLength(50); //shouldn't these be on resources instead?

        //        topologySettings.Resources().Queues().AutoDeleteOnIdle(TimeSpan.FromDays(10); // change to properties topologySettings.Resources.Queues.AutoDeleteOnIdle =  true
        //        topologySettings.Resources().Queues().DescriptionFactory((path, settings) => new QueueDescription(path)); //Change to SetDescriptionFactory
        //        topologySettings.Resources().Queues().DefaultMessageTimeToLive(TimeSpan.FromHours(1));
        //        topologySettings.Resources().Queues().DuplicateDetectionHistoryTimeWindow(TimeSpan.FromHours(1));
        //        topologySettings.Resources().Queues().EnableBatchedOperations(true);
        //        topologySettings.Resources().Queues().EnableDeadLetteringOnMessageExpiration(true);
        //        topologySettings.Resources().Queues().EnableExpress(true);
        //        topologySettings.Resources().Queues().EnablePartitioning(true);
        //        topologySettings.Resources().Queues().ForwardDeadLetteredMessagesTo("dlq");
        //        topologySettings.Resources().Queues().ForwardDeadLetteredMessagesTo(path => true, "dlq");
        //        topologySettings.Resources().Queues().LockDuration(TimeSpan.FromMinutes(1));
        //        topologySettings.Resources().Queues().AutoDeleteOnIdle(TimeSpan.FromHours(24));
        //        topologySettings.Resources().Queues().MaxDeliveryCount(5);
        //        topologySettings.Resources().Queues().MaxSizeInMegabytes(1024);
        //        topologySettings.Resources().Queues().RequiresSession(true);
        //        topologySettings.Resources().Queues().SupportOrdering(true);

        //        topologySettings.Resources().Topics().DescriptionFactory((path, settings) => new TopicDescription(path)); //Change to SetDescriptionFactory
        //        all other topic properties 

        //        topologySettings.Resources().Subscriptions().DescriptionFactory((topicPath, subscriptionName, settings) => new SubscriptionDescription(topicPath, subscriptionName)); //Change to SetDescriptionFactory
        //        all other subscription properties 

        var endpoint = await Endpoint.Start(configuration);

        Console.WriteLine("Press ESC to exit");
        Console.WriteLine("listening to messages");
        while (Console.ReadKey().Key != ConsoleKey.Escape)
        {
        }

        await endpoint.Stop();
    }

    class NoOpOversizedBrokeredMessagesHandler : IHandleOversizedBrokeredMessages
    {
        public Task Handle(BrokeredMessage brokeredMessage)
        {
            return Task.FromResult(0);
        }
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