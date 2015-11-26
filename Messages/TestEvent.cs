namespace Messages
{
    using NServiceBus;
    public interface TestEvent : IEvent
    {
        string Data { get; set; }
    }
}