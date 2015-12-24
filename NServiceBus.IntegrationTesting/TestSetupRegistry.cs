namespace NServiceBus.IntegrationTesting
{
    using System;
    using System.Collections.Concurrent;

    public static class TestSetupRegistry
    {
        private readonly static ConcurrentDictionary<Guid, ITestSetup> Setups = new ConcurrentDictionary<Guid, ITestSetup>();

        internal static void Register(ITestSetup testSetup)
        {
            Setups.TryAdd(testSetup.Id, testSetup);
        }

        internal static void Unregister(ITestSetup testSetup)
        {
            ITestSetup removed;
            Setups.TryRemove(testSetup.Id, out removed);
        }

        public static void Dispatch(object message)
        {
            foreach (var tests in Setups.Values)
            {
                tests.Handle(message);
            }
        }
    }
}