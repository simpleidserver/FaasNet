using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Sink.Stores;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FaasNet.EventMesh.Sink.VpnBridge
{
    public class VpnBridgeSinkJob : BaseSinkJob
    {
        private readonly VpnBridgeSinkOptions _options;
        private System.Timers.Timer _listenBridgeVpnTimer;
        private CancellationTokenSource _tokenSource;
        private ICollection<ListenRecord> _listenRecords;

        public VpnBridgeSinkJob(IOptions<VpnBridgeSinkOptions> options, ISubscriptionStore subscriptionStore, IOptions<SinkOptions> seedOptions) : base(subscriptionStore, seedOptions) 
        {
            _options = options.Value;
        }

        protected override string JobId => _options.JobId;

        protected override Task Subscribe(CancellationToken cancellationToken)
        {
            _listenRecords = new List<ListenRecord>();
            _listenBridgeVpnTimer = new System.Timers.Timer(_options.GetBridgeServerLstIntervalMS);
            _listenBridgeVpnTimer.Elapsed += async (o, e) => await HandleTimer(o, e);
            _listenBridgeVpnTimer.AutoReset = false;
            _listenBridgeVpnTimer.Start();
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            return Task.CompletedTask;
        }

        protected override Task Unsubscribe(CancellationToken cancellationToken)
        {
            foreach(var listenRecord in _listenRecords)
            {
                foreach(var subscription in listenRecord.Subscriptions)
                {
                    subscription.Close();
                }
            }

            _listenBridgeVpnTimer.Stop();
            _tokenSource.Cancel();
            return Task.CompletedTask;
        }

        private async Task HandleTimer(object source, ElapsedEventArgs e)
        {
            var allVpnBridges = await GetAllVpnBridges(_tokenSource.Token);
            if (!allVpnBridges.Any())
            {
                _listenBridgeVpnTimer.Start();
                return;
            }

            var bridgeNotYetSubscribed = allVpnBridges.Where(br => !_listenRecords.Any(r => r.Bridges.Any(b => b.TargetVpn == br.TargetVpn && b.TargetUrn == br.TargetUrn && b.TargetPort == br.TargetPort)));
            if (!bridgeNotYetSubscribed.Any())
            {
                _listenBridgeVpnTimer.Start();
                return;
            }

            var listenRecord = new ListenRecord { Bridges = bridgeNotYetSubscribed.ToList() };
#pragma warning disable 4014
            Task.Run(async () => await ListenBridges(listenRecord));
#pragma warning restore 4014
            _listenRecords.Add(listenRecord);
            _listenBridgeVpnTimer.Start();
        }

        private Task<IEnumerable<BridgeServerResponse>> GetAllVpnBridges(CancellationToken cancellationToken)
        {
            var eventMeshClient = new EventMeshClient(Options.EventMeshUrl, Options.EventMeshPort);
            return eventMeshClient.GetAllBridges(cancellationToken);
        }

        private async Task ListenBridges(ListenRecord listenRecord)
        {
            foreach(var bridgeServer in listenRecord.Bridges)
            {
                var sourceEventMeshClient = new EventMeshClient(bridgeServer.TargetUrn, bridgeServer.TargetPort);
                var subSession = await sourceEventMeshClient.CreateSubSession(bridgeServer.TargetVpn, bridgeServer.TargetClientId, null, _tokenSource.Token);
                var subscriptionResult = await subSession.PersistedSubscribe("*", _options.EventMeshServerGroupId, async (cloudEvt) =>
                {
                    var targetEventMeshClient = new EventMeshClient(Options.EventMeshUrl, Options.EventMeshPort);
                    var pubSession = await targetEventMeshClient.CreatePubSession(bridgeServer.SourceVpn, Options.ClientId, null, _tokenSource.Token);
                    await pubSession.Publish(cloudEvt.Type, cloudEvt, _tokenSource.Token);
                }, _tokenSource.Token);
                listenRecord.Subscriptions.Add(subscriptionResult);
            }
        }

        private class ListenRecord
        {
            public ListenRecord()
            {
                Bridges = new List<BridgeServerResponse>();
                Subscriptions = new List<Client.SubscriptionResult>();
            }

            public IEnumerable<BridgeServerResponse> Bridges { get; set; }
            public ICollection<Client.SubscriptionResult> Subscriptions { get; set; }
        }
    }
}