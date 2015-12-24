using NServiceBus;

namespace NServiceBus.IntegrationTesting
{
    public class TestHandler : IHandleMessages<object>
    {
        public void Handle(object message)
        {
            TestSetupRegistry.Dispatch(message);
        }
    }
}