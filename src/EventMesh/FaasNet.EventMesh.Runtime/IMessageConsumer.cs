using FaasNet.EventMesh.Runtime.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime
{
    public interface IMessageConsumer : IDisposable
    {
        string BrokerName { get; }
        Task Start(CancellationToken cancellationToken);
        Task Stop(CancellationToken cancellationToken);
        Task Subscribe(string topic, Models.Client client, string sessionId, CancellationToken cancellationToken);
        Task Unsubscribe(string topic, Models.Client client, string sessionId, CancellationToken cancellationToken);
        void Commit(string topicName, Models.Client client, string sessionId, int nbEvts);
        event EventHandler<CloudEventArgs> CloudEventReceived;
    }
}
