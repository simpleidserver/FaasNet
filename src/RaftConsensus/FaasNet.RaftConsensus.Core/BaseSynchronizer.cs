using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FaasNet.RaftConsensus.Core
{
    public abstract class BaseSynchronizer
    {
        private readonly IClusterStore _clusterStore;
        private CancellationTokenSource _cancellationTokenSource;
        private UdpClient _udpClient;

        public BaseSynchronizer(IOptions<ConsensusNodeOptions> nodeOptions, IClusterStore clusterStore)
        {
            _clusterStore = clusterStore;
            Timer = new System.Timers.Timer(nodeOptions.Value.SynchronizeTimerMS);
            Timer.Elapsed += HandleTimer;
            Timer.AutoReset = false;
        }

        protected System.Timers.Timer Timer { get; private set; }
        protected abstract string EntityType { get; }
        protected abstract int EntityVersion { get; }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _udpClient = new UdpClient();
            Timer.Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _udpClient.Close();
            Timer.Stop();
        }

        public async Task Ingest()
        {

        }

        private async void HandleTimer(object sender, ElapsedEventArgs e)
        {
            var writeCtx = new WriteBufferContext();
            var pkg = GossipPackageRequestBuilder.Heartbeat(EntityType, EntityVersion);
            pkg.Serialize(writeCtx);
            var resultPayload = writeCtx.Buffer.ToArray();
            var nodes = await _clusterStore.GetAllNodes(_cancellationTokenSource.Token);
            foreach(var node in nodes)
            {
                var ipEdp = new IPEndPoint(ConsensusClient.ResolveIPAddress(node.Url), node.Port);
                await _udpClient.SendAsync(resultPayload, resultPayload.Count(), ipEdp).WithCancellation(_cancellationTokenSource.Token);
            }

            Timer.Start();
        }
    }
}