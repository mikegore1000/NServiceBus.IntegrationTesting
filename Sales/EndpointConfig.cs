
namespace Sales
{
    using NServiceBus;

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher
    {
        public void Init()
        {
            Configure.With()
                .InMemorySubscriptionStorage();
        }
    }
}
