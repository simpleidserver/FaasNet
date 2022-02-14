using EventMesh.Runtime.Events;
using EventMesh.Runtime.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime
{
    public interface IMessageConsumer : IDisposable
    {
        Task Start(CancellationToken cancellationToken);
        Task Stop(CancellationToken cancellationToken);
        Task Subscribe(string topic, Client client, string sessionId, CancellationToken cancellationToken);
        Task Unsubscribe(string topic, Client client, string sessionId, CancellationToken cancellationToken);
        event EventHandler<CloudEventArgs> CloudEventReceived;
    }
}
