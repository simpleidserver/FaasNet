using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Sink.Stores;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Sink
{
    public abstract class BaseSinkJob : ISinkJob
    {
        private CancellationTokenSource _tokenSource;
        private SinkOptions _sinkOptions;

        public BaseSinkJob(ISubscriptionStore subscriptionStore, IOptions<SinkOptions> sinkOptions)
        {
            SubscriptionStore = subscriptionStore;
            _sinkOptions = sinkOptions.Value;
        }

        public bool IsRunning { get; private set; }
        public ISubscriptionStore SubscriptionStore { get; private set; }
        public SinkOptions Options => _sinkOptions;

        public async Task Start(CancellationToken cancellationToken)
        {
            if (IsRunning) throw new InvalidOperationException("Seed is already running");
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await Subscribe(_tokenSource.Token);
            IsRunning = true;
        }

        public async Task Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("Seed is not running");
            _tokenSource.Cancel();
            await Unsubscribe(_tokenSource.Token);
            IsRunning = false;
        }

        protected async Task Publish(string topic, CloudEvent cloudEvt, CancellationToken cancellationToken)
        {
            var evtMeshClient = new EventMeshClient(_sinkOptions.EventMeshUrl, _sinkOptions.EventMeshPort);
            var pubSession = await evtMeshClient.CreatePubSession(_sinkOptions.Vpn, _sinkOptions.ClientId, null, cancellationToken);
            await pubSession.Publish(topic, cloudEvt, cancellationToken);
        }

        protected abstract Task Subscribe(CancellationToken cancellationToken);
        protected abstract Task Unsubscribe(CancellationToken cancellationToken);
        protected abstract string JobId { get; }
    }
}
