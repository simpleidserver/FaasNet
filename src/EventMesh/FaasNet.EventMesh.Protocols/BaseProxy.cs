using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols
{
    public abstract class BaseProxy : IProxy
    {
        public bool IsRunning { get; private set; }
        protected CancellationTokenSource TokenSource { get; private set; }

        public Task Start()
        {
            if (IsRunning) throw new InvalidOperationException("The proxy is already running");
            IsRunning = true;
            TokenSource = new CancellationTokenSource();
            Init();
            return Task.CompletedTask;
        }

        public void Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("The proxy is not running");
            IsRunning = false;
            TokenSource.Cancel();
            Shutdown();
        }

        protected abstract void Init();
        protected abstract void Shutdown();
    }
}
