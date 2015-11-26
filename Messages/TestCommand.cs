namespace Messages
{
    using NServiceBus;

    public class TestCommand : ICommand
    {
        public string Data { get; set; }
    }
}