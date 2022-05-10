using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Client.Messages.Consensus;
using FaasNet.RaftConsensus.Client.Messages.Gossip;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FaasNet.RaftConsensus.Core
{
    public interface INodeHost
    {
        event EventHandler<EventArgs> NodeStarted;
        string NodeId { get; }
        int Port { get; }
        bool IsStarted { get; }
        BlockingCollection<IPeerHost> Peers { get; }
        BlockingCollection<UnreachableClusterNode> UnreachableClusterNodes { get; }
        INodeStateStore NodeStateStore { get; }
        Task Start(CancellationToken cancellationToken);
        Task Stop();
    }

    public abstract class BaseNodeHost : INodeHost
    {
        private readonly IPeerStore _peerStore;
        private readonly IPeerHostFactory _peerHostFactory;
        private readonly INodeStateStore _nodeStateStore;
        private readonly IClusterStore _clusterStore;
        private readonly ConsensusPeerOptions _options;
        private readonly UdpClient _proxyClient;
        private System.Timers.Timer _gossipTimer;
        private BlockingCollection<IPeerHost> _peers;
        private string _nodeId;
        private bool _isStarted = false;
        private BlockingCollection<UnreachableClusterNode> _clusterNodes;

        public BaseNodeHost(IPeerStore peerStore, IPeerHostFactory peerHostFactory, INodeStateStore nodeStateStore, IClusterStore clusterStore, ILogger<BaseNodeHost> logger, IOptions<ConsensusPeerOptions> options)
        {
            _peerStore = peerStore;
            _peerHostFactory = peerHostFactory;
            _nodeStateStore = nodeStateStore;
            _clusterStore = clusterStore;
            Logger = logger;
            _options = options.Value;
            _proxyClient = new UdpClient();
            _nodeId = Guid.NewGuid().ToString();
        }

        public BlockingCollection<IPeerHost> Peers => _peers;
        public BlockingCollection<UnreachableClusterNode> UnreachableClusterNodes => _clusterNodes;
        public INodeStateStore NodeStateStore => _nodeStateStore;
        public IClusterStore ClusterStore => _clusterStore;
        public bool IsStarted => _isStarted;
        public int Port => _options.Port;
        public bool IsRunning { get; private set; }
        public UdpClient UdpServer { get; private set; }
        public ILogger<BaseNodeHost> Logger { get; private set; }
        public string NodeId => _nodeId;
        public event EventHandler<EventArgs> NodeStarted;
        public event EventHandler<EventArgs> NodeStopped;
        protected CancellationTokenSource TokenSource { get; private set; }

        public async Task Start(CancellationToken cancellationToken)
        {
            if (IsRunning) throw new InvalidOperationException("The node is already running");
            _peers = new BlockingCollection<IPeerHost>();
            _clusterNodes = new BlockingCollection<UnreachableClusterNode>();
            TokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await RunConsensusPeers(cancellationToken);
            StartGossipTimer();
            UdpServer = new UdpClient(new IPEndPoint(IPAddress.Any, _options.Port));
#pragma warning disable 4014
            Task.Run(async () => await InternalRun(), cancellationToken);
#pragma warning restore 4014
            IsRunning = true;
        }

        public Task Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("The node is not running");
            StopConsensusPeers();
            StopGossipTimer();
            IsRunning = false;
            _proxyClient.Close();
            UdpServer.Close();
            _isStarted = false;
            return Task.CompletedTask;
        }

        protected async Task InternalRun()
        {
            try
            {
                if (NodeStarted != null) NodeStarted(this, new EventArgs());
                _isStarted = true;
                while (true)
                {
                    TokenSource.Token.ThrowIfCancellationRequested();
                    await HandleUDPPackage();
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            if (NodeStopped != null) NodeStopped(this, new EventArgs());
        }

        protected async Task HandleUDPPackage()
        {
            try
            {
                var udpResult = await UdpServer.ReceiveAsync().WithCancellation(TokenSource.Token);
                if (await TryHandleConsensusRequest(udpResult)) return;
                if (await TryHandleGossipRequest(udpResult)) return;
                await HandleUDPPackage(udpResult, TokenSource.Token);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        protected abstract Task HandleUDPPackage(UdpReceiveResult udpResult, CancellationToken cancellationToken);

        #region Consensus

        private async Task<bool> TryHandleConsensusRequest(UdpReceiveResult udpResult)
        {
            var bufferContext = new ReadBufferContext(udpResult.Buffer.ToArray());
            var consensusPackage = ConsensusPackage.Deserialize(bufferContext);
            if (consensusPackage == null) return false;
            var peerHost = _peers.First(p => p.Info.TermId == consensusPackage.Header.TermId);
            await _proxyClient.SendAsync(udpResult.Buffer, udpResult.Buffer.Count(), peerHost.UdpServerEdp);
            return true;
        }

        private async Task RunConsensusPeers(CancellationToken cancellationToken)
        {
            var peerInfoLst = await _peerStore.GetAll(cancellationToken);
            foreach (var peerInfo in peerInfoLst)
            {
                var peerHost = _peerHostFactory.Build();
                await peerHost.Start(_nodeId, peerInfo, cancellationToken);
                _peers.Add(peerHost);
            }
        }

        private void StopConsensusPeers()
        {
            Parallel.ForEach(_peers, async (peer) =>
            {
                await peer.Stop();
            });
        }

        #endregion

        #region Gossip

        private async Task<bool> TryHandleGossipRequest(UdpReceiveResult udpResult)
        {
            var bufferContext = new ReadBufferContext(udpResult.Buffer.ToArray());
            var gossipPackage = GossipPackage.Deserialize(bufferContext);
            if (gossipPackage == null) return false;
            var packageResult = await HandleGossipRequest(gossipPackage);
            if(packageResult == null) return true;
            using(var client = new GossipClient(gossipPackage.Header.SourceUrl, gossipPackage.Header.SourcePort))
            {
                await client.Send(packageResult, cancellationToken: TokenSource.Token);
            }

            return true;
        }

        private void StartGossipTimer()
        {
            _gossipTimer = new System.Timers.Timer(_options.GossipTimerMS);
            _gossipTimer.Elapsed += async (o, e) => await BroadcastGossipHeartbeat(o, e);
            _gossipTimer.AutoReset = false;
            _gossipTimer.Start();
        }

        private void StopGossipTimer()
        {
            _gossipTimer.Stop();
        }

        private async Task BroadcastGossipHeartbeat(object source, ElapsedEventArgs e)
        {
            var nodes = await _clusterStore.GetAllNodes(TokenSource.Token);
            nodes = nodes.Where(n => n.Port != _options.Port || n.Url != _options.Url);
            var totalNodes = nodes.Count();
            var nbNodes = _options.GossipMaxNodeBroadcast;
            if(totalNodes < nbNodes) nbNodes = totalNodes;
            var rnd = new Random();
            for(var i = 0; i < nbNodes; i++)
            {
                var rndNodeIndex = rnd.Next(0, totalNodes);
                var selectedNode = nodes.ElementAt(rndNodeIndex);
                if (_clusterNodes.Any(n => n.Node.Url == selectedNode.Url && n.Node.Port == selectedNode.Port && n.ReactivationDateTime > DateTime.UtcNow)) continue;
                try
                {
                    using (var client = new GossipClient(selectedNode.Url, selectedNode.Port))
                    {
                        await client.Heartbeat(_options.Url, _options.Port, _options.GossipTimeoutHeartbeatMS, TokenSource.Token);
                    }
                }
                catch(Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    var clusterNode = _clusterNodes.FirstOrDefault(c => c.Node.Port == selectedNode.Port && c.Node.Url == selectedNode.Url);
                    if (clusterNode == null) _clusterNodes.Add(new UnreachableClusterNode(selectedNode, DateTime.UtcNow.AddMilliseconds(_options.GossipClusterNodeDeactivationDurationMS)));
                    else clusterNode.ReactivationDateTime = DateTime.UtcNow.AddMilliseconds(_options.GossipClusterNodeDeactivationDurationMS);
                }
            }

            StartGossipTimer();
        }

        private Task<GossipPackage> HandleGossipRequest(dynamic request)
        {
            return HandleGossipRequest(request);
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipHeartbeatRequest request)
        {
            var states = await _nodeStateStore.GetAllLastEntityTypes(TokenSource.Token);
            var dic = states.ToDictionary(s => s.EntityType, s => s.EntityVersion);
            return GossipPackageResultBuilder.Heartbeat(_options.Url, _options.Port, dic);
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipHeartbeatResult request)
        {
            var states = await _nodeStateStore.GetAllLastEntityTypes(TokenSource.Token);
            var stateToBeSynced = states.Where(s => request.States.Any(st => st.EntityType == s.EntityType && s.EntityVersion < st.EntityVersion))
                .ToDictionary(s => s.EntityType, s => s.EntityVersion + 1);
            var missingStates = request.States.Where(rs => !states.Any(s => s.EntityType == rs.EntityType));
            foreach (var missingState in missingStates) stateToBeSynced.Add(missingState.EntityType, missingState.EntityVersion);
            if (!stateToBeSynced.Any()) return null;
            return GossipPackageRequestBuilder.Sync(_options.Url, _options.Port, stateToBeSynced);
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipSyncStateRequest request)
        {
            var stateValues = (await _nodeStateStore.GetAllSpecificEntityTypes(request.States.Select(s => (EntityType: s.EntityType, EntityVersion: s.EntityVersion)).ToList(), TokenSource.Token))
                .ToDictionary(s => s.EntityType, s => (Version: s.EntityVersion, Value: s.Value));
            if (!stateValues.Any()) return null;
            return GossipPackageResultBuilder.Sync(_options.Url, _options.Port, stateValues);
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipSyncStateResult request)
        {
            var existingValues = await _nodeStateStore.GetAllSpecificEntityTypes(request.States.Select(s => (EntityType: s.EntityType, EntityVersion: s.EntityVersion)).ToList(), TokenSource.Token);
            var missingValues = request.States.Where(s => !existingValues.Any(ev => ev.EntityType == s.EntityType && ev.EntityVersion >= s.EntityVersion));
            foreach (var missingValue in missingValues) _nodeStateStore.Add(NodeState.Create(missingValue.EntityType, missingValue.EntityId, missingValue.Value, missingValue.EntityVersion));
            await _nodeStateStore.SaveChanges(TokenSource.Token);
            return null;
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipUpdateNodeStateRequest request)
        {
            var nodeState = await _nodeStateStore.GetLastEntityType(request.EntityType, TokenSource.Token);
            if(nodeState == null) _nodeStateStore.Add(NodeState.Create(request.EntityType, request.EntityId, request.Value));
            else nodeState.Update(request.Value);
            await _nodeStateStore.SaveChanges(TokenSource.Token);
            return null;
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipJoinNodeRequest request)
        {
            var cluster = await _clusterStore.GetNode(request.Url, request.Port, TokenSource.Token);
            if (cluster == null) 
            {
                await _clusterStore.AddNode(new ClusterNode { Port = request.Port, Url = request.Url }, TokenSource.Token);
                var allNodes = await _clusterStore.GetAllNodes(TokenSource.Token);
                using (var gossipClient = new GossipClient(request.Url, request.Port))
                {
                    await gossipClient.UpdateClusterNodes(_options.Url, _options.Port, allNodes.Select(n => new ClusterNodeMessage { Port = n.Port, Url = n.Url }).ToList(), TokenSource.Token);
                }
            }

            return null;
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipUpdateClusterRequest request)
        {
            foreach(var node in request.Nodes)
            {
                await _clusterStore.AddNode(new ClusterNode { Port = node.Port, Url = node.Url }, TokenSource.Token);
            }

            return null;
        }

        #endregion
    }

    public class UnreachableClusterNode
    {
        public UnreachableClusterNode(ClusterNode node, DateTime reactivationDateTime)
        {
            Node = node;
            ReactivationDateTime = reactivationDateTime;
        }

        public ClusterNode Node { get; private set; }
        public DateTime ReactivationDateTime { get; set; }
    }
}
