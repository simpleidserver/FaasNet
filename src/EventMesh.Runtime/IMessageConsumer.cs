using EventMesh.Runtime.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime
{
    public interface IMessageConsumer : IDisposable
    {
        Task Start(CancellationToken cancellationToken);
        Task Stop(CancellationToken cancellationToken);
        Task Subscribe(string topic, CancellationToken cancellationToken);
        Task Unsubscribe(string topic, CancellationToken cancellationToken);
        event EventHandler<CloudEventArgs> CloudEventReceived;
    }
}
