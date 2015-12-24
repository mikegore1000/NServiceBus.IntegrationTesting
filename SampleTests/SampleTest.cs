namespace NServiceBusIntegrationTesting
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using NServiceBus;
    using NUnit.Framework;
    using NServiceBus.IntegrationTesting;

    [TestFixture]
    public class Given_An_Order_Is_Placed
    {
        private IBus _bus;

        // Setting up the bus is expensive, so do it once for the entire fixture
        [TestFixtureSetUp]
        public void BusSetup()
        {
            var bus = Configure.With()
                .DefaultBuilder()
                .UseTransport<Msmq>()
                .InMemorySubscriptionStorage()
                .PurgeOnStartup(true)
                .UnicastBus()
                .CreateBus();

            bus.Subscribe<OrderPlaced>();
            bus.Subscribe<OrderBilled>();

            _bus = bus.Start();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _bus.Unsubscribe<OrderPlaced>();
            _bus.Unsubscribe<OrderBilled>();
        }

        [Test]
        public async Task Then_The_Order_Is_Placed()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var setup = new TestSetup<OrderPlaced>(x => x.OrderId == orderId, TimeSpan.FromSeconds(5));

            // Must create the TestSetup instances before sending messages
            // otherwise there is a risk (albeit incredibly small) that
            // the test completes before the instance is created.
            var msg = new PlaceOrder(orderId);
            _bus.Send(msg);

            // Act
            var orderPlaced = await setup.Task;

            // Assert
            Assert.That(orderPlaced, Is.Not.Null, "expected a message to be returned.");
        }

        [Test]
        public async Task Then_The_Order_Is_Billed()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var setup = new TestSetup<OrderBilled>(x => x.OrderId == orderId, TimeSpan.FromSeconds(5));

            // Must create the TestSetup instances before sending messages
            // otherwise there is a risk (albeit incredibly small) that
            // the test completes before the instance is created.
            var msg = new PlaceOrder(orderId);
            _bus.Send(msg);

            // Act
            var orderBilled = await setup.Task;

            // Assert
            Assert.That(orderBilled, Is.Not.Null, "expected a message to be returned.");
        }
    }
}