namespace NServiceBus.IntegrationTesting
{
    using System;
    using Tasks = System.Threading.Tasks;

    public interface ITestSetup
    {
        Guid Id { get; }
        bool CanHandle(object message);
        void Handle(object message);
    }

    public class TestSetup<TIncoming> : ITestSetup where TIncoming : class
    {
        private readonly Tasks.TaskCompletionSource<TIncoming> _taskCompletionSource;
        private readonly Func<TIncoming, bool> _messageFunc;

        public TestSetup(Func<TIncoming, bool> matchingFunc)
            : this(matchingFunc, TimeSpan.FromSeconds(30))
        {
            
        }

        public TestSetup(Func<TIncoming, bool> matchingFunc, TimeSpan within)
        {
            Id = Guid.NewGuid();
            _messageFunc = matchingFunc;
            _taskCompletionSource = new Tasks.TaskCompletionSource<TIncoming>();

            TestSetupRegistry.Register(this);

            Tasks.Task.Delay(within).ContinueWith(t =>
            {
                if (!_taskCompletionSource.Task.IsCompleted)
                {
                    _taskCompletionSource.SetException(
                        new Exception(string.Format("Failed to recieve message within {0}", within)));
                }
            });
        }

        public Guid Id { get; private set; }

        public bool CanHandle(object message)
        {
            var incoming = message as TIncoming;

            if (incoming == null)
                return false;

            return _messageFunc(incoming);
        }

        public void Handle(object message)
        {
            if (CanHandle(message))
            {
                TestSetupRegistry.Unregister(this);
                _taskCompletionSource.SetResult(message as TIncoming);
            }
        }
         
        public Tasks.Task<TIncoming> Task
        {
            get { return _taskCompletionSource.Task; }
        }
    }
}
