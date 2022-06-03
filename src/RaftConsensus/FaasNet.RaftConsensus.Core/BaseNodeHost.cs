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
using System.Text.Json;
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
        private readonly IPeerInfoStore _peerInfoStore;
        private readonly IPeerHostFactory _peerHostFactory;
        private readonly INodeStateStore _nodeStateStore;
        private readonly IClusterStore _clusterStore;
        private readonly ConsensusNodeOptions _options;
        private readonly UdpClient _proxyClient;
        private System.Timers.Timer _gossipTimer;
        private BlockingCollection<IPeerHost> _peers;
        private string _nodeId;
        private bool _isStarted = false;
        private BlockingCollection<UnreachableClusterNode> _clusterNodes;

        public BaseNodeHost(IPeerStore peerStore, IPeerInfoStore peerInfoStore, IPeerHostFactory peerHostFactory, INodeStateStore nodeStateStore, IClusterStore clusterStore, ILogger<BaseNodeHost> logger, IOptions<ConsensusNodeOptions> options)
        {
            _peerStore = peerStore;
            _peerInfoStore = peerInfoStore;
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
        public string Url => _options.Url;
        public UdpClient UdpServer { get; private set; }
        public bool IsRunning { get; private set; }
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
            _proxyClient.Close();
            UdpServer.Close();
            IsRunning = false;
            _isStarted = false;
            return Task.CompletedTask;
        }

        protected async Task InternalRun()
        {
            try
            {
                if (NodeStarted != null) NodeStarted(this, new EventArgs());
                _isStarted = true;
                await _clusterStore.SelfRegister(new ClusterNode { Url = _options.Url, Port = _options.Port }, CancellationToken.None);
                while (true)
                {
                    TokenSource.Token.ThrowIfCancellationRequested();
                    await HandlePackage();
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            if (NodeStopped != null) NodeStopped(this, new EventArgs());
        }

        protected async Task HandlePackage()
        {
            try
            {
                var udpResult = await UdpServer.ReceiveAsync().WithCancellation(TokenSource.Token);
                if (await TryHandleConsensusRequest(udpResult)) return;
                if (await TryHandleGossipRequest(udpResult)) return;
                await HandlePackage(udpResult, TokenSource.Token);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        protected abstract Task HandlePackage(UdpReceiveResult udpResult, CancellationToken cancellationToken);

        #region Consensus

        private async Task<bool> TryHandleConsensusRequest(UdpReceiveResult transportResult)
        {
            var bufferContext = new ReadBufferContext(transportResult.Buffer.ToArray());
            var consensusPackage = ConsensusPackage.Deserialize(bufferContext);
            if (consensusPackage == null) return false;
            var peerHost = _peers.FirstOrDefault(p => p.Info.TermId == consensusPackage.Header.TermId);
            if (peerHost == null) return true;
            await _proxyClient.SendAsync(transportResult.Buffer, transportResult.Buffer.Count(), peerHost.UdpServerEdp);
            return true;
        }

        private async Task RunConsensusPeers(CancellationToken cancellationToken)
        {
            var peerInfoLst = await _peerInfoStore.GetAll(cancellationToken);
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

        private async Task<bool> TryHandleGossipRequest(UdpReceiveResult transportResult)
        {
            var bufferContext = new ReadBufferContext(transportResult.Buffer.ToArray());
            var gossipPackage = GossipPackage.Deserialize(bufferContext);
            if (gossipPackage == null) return false;
            var packageResult = await HandleGossipRequest(gossipPackage);
            if(packageResult == null) return true;
            if(!string.IsNullOrWhiteSpace(gossipPackage.Header.SourceUrl))
            {
                using (var client = new GossipClient(gossipPackage.Header.SourceUrl, gossipPackage.Header.SourcePort))
                {
                    await client.Send(packageResult, cancellationToken: TokenSource.Token);
                }
            }
            else
            {
                using (var client = new GossipClient(transportResult.RemoteEndPoint))
                {
                    await client.Send(packageResult, cancellationToken: TokenSource.Token);
                }
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
            nodes = nodes.Where(n => n.Port != Port || n.Url != Url);
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
                        await client.Heartbeat(Url, Port, _options.GossipTimeoutHeartbeatMS, TokenSource.Token);
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
            return GossipPackageResultBuilder.Heartbeat(Url, Port, dic);
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipHeartbeatResult request)
        {
            var states = await _nodeStateStore.GetAllLastEntityTypes(TokenSource.Token);
            var stateToBeSynced = states.Where(s => request.States.Any(st => st.EntityType == s.EntityType && s.EntityVersion < st.EntityVersion))
                .ToDictionary(s => s.EntityType, s => s.EntityVersion + 1);
            var missingStates = request.States.Where(rs => !states.Any(s => s.EntityType == rs.EntityType));
            foreach (var missingState in missingStates) stateToBeSynced.Add(missingState.EntityType, missingState.EntityVersion);
            if (!stateToBeSynced.Any()) return null;
            return GossipPackageRequestBuilder.Sync(Url, Port, stateToBeSynced);
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipSyncStateRequest request)
        {
            var stateValues = (await _nodeStateStore.GetAllSpecificEntityTypes(request.States.Select(s => (EntityType: s.EntityType, EntityVersion: s.EntityVersion)).ToList(), TokenSource.Token))
                .ToDictionary(s => s.EntityType, s => (Version: s.EntityVersion, Value: s.Value));
            if (!stateValues.Any()) return null;
            return GossipPackageResultBuilder.Sync(Url, Port, stateValues);
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipSyncStateResult request)
        {
            var existingValues = await _nodeStateStore.GetAllSpecificEntityTypes(request.States.Select(s => (EntityType: s.EntityType, EntityVersion: s.EntityVersion)).ToList(), TokenSource.Token);
            var missingValues = request.States.Where(s => !existingValues.Any(ev => ev.EntityType == s.EntityType && ev.EntityVersion >= s.EntityVersion));
            foreach (var missingValue in missingValues)
            {
                _nodeStateStore.Add(NodeState.Create(missingValue.EntityType, missingValue.EntityId, missingValue.Value, missingValue.EntityVersion));
                if (missingValue.EntityType == StandardEntityTypes.Peer)
                {
                    var peer = JsonSerializer.Deserialize<Peer>(missingValue.Value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await StartPeer(peer.TermId);
                }
            }

            return null;
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipUpdateNodeStateRequest request)
        {
            var nodeState = await _nodeStateStore.GetLastEntityType(request.EntityType, TokenSource.Token);
            if(nodeState == null) _nodeStateStore.Add(NodeState.Create(request.EntityType, request.EntityId, request.Value));
            else nodeState.Update(request.Value);
            return null;
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipAddPeerRequest request)
        {
            await StartPeer(request.TermId);
            await AddPeer(request.TermId);
            return null;
        }

        private async Task<GossipPackage> HandleGossipRequest(GossipGetClusterNodesRequest request)
        {
            var allNodes = await _clusterStore.GetAllNodes(TokenSource.Token);
            return GossipPackageResultBuilder.GetClusterNodes(_options.Url, _options.Port, allNodes.Select(n => new ClusterNodeResult
            {
                Port = n.Port,
                Url = n.Url
            }).ToList());
        }

        protected async Task AddPeer(string termId)
        {
            await _peerStore.Add(new Peer { TermId = termId }, TokenSource.Token);
        }

        protected async Task<IPeerHost> StartPeer(string termId)
        {
            var peerInfo = new PeerInfo { TermId = termId };
            var peerHost = _peerHostFactory.Build();
            await peerHost.Start(_nodeId, peerInfo, TokenSource.Token);
            _peers.Add(peerHost);
            _peerInfoStore.Add(peerInfo);
            return peerHost;
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
