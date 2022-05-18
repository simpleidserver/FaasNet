using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Seed.Stores;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Seed
{
    public abstract class BaseSeedJob
    {
        private CancellationTokenSource _tokenSource;
        private SeedOptions _seedOptions;

        public BaseSeedJob(ISubscriptionStore subscriptionStore, IOptions<SeedOptions> seedOptions)
        {
            SubscriptionStore = subscriptionStore;
            _seedOptions = seedOptions.Value;
        }

        public bool IsRunning { get; private set; }
        public ISubscriptionStore SubscriptionStore { get; private set; }

        public async Task Start(CancellationToken cancellationToken)
        {
            if (IsRunning) throw new InvalidOperationException("Seed is already running");
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var offset = await SubscriptionStore.GetOffset(JobId, cancellationToken);
            await Subscribe(offset, _tokenSource.Token);
            IsRunning = true;
        }

        public async void Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("Seed is not running");
            _tokenSource.Cancel();
            await Unsubscribe(_tokenSource.Token);
            IsRunning = false;
        }

        protected async Task Publish(string topic, CloudEvent cloudEvt, CancellationToken cancellationToken)
        {
            var evtMeshClient = new EventMeshClient(_seedOptions.EventMeshUrl, _seedOptions.EventMeshPort);
            var pubSession = await evtMeshClient.CreatePubSession(_seedOptions.Vpn, _seedOptions.ClientId, cancellationToken);
            await pubSession.Publish(topic, cloudEvt, cancellationToken);
        }

        protected abstract Task Subscribe(int offset, CancellationToken cancellationToken);
        protected abstract Task Unsubscribe(CancellationToken cancellationToken);
        protected abstract string JobId { get; }
    }
}
