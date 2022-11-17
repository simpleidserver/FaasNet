using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FaasNet.EventMesh.Performance
{
    public class EventMeshPartitionedPeerPool
    {
        private readonly ConcurrentBag<string> _electedPartitionNames = new ConcurrentBag<string>();
        private static EventMeshPartitionedPeerPool _instance = null!;
        private readonly ICollection<WaitElectedPartitionTask> _electedPartitionTasks = new List<WaitElectedPartitionTask>();
        private readonly ConcurrentBag<IPeerHost> _peers = new ConcurrentBag<IPeerHost>();
        private readonly ConcurrentBag<ClusterPeer> _inMemoryCluster = new ConcurrentBag<ClusterPeer>();

        private EventMeshPartitionedPeerPool() { }

        public void AddPartitionedNode(int port, int peerPort)
        {
            var peer = BuildPartitionedNode(port, _inMemoryCluster, peerPort, HandleElectedPartitionName);
            _peers.Add(peer);

            void HandleElectedPartitionName(string name)
            {
                foreach (var electedPartitionTask in _electedPartitionTasks)
                    electedPartitionTask.ElectPartition(name);
            }
        }

        public async Task Start()
        {
            foreach (var peer in _peers) await peer.Start();
        }

        public Task WaitAllStandardPartitionsAreLaunched() => WaitPartitions(PartitionNames.ALL);

        public Task WaitPartition(string partitionName) => WaitPartitions(new[] { partitionName });

        public Task WaitPartitions(IEnumerable<string> partitions)
        {
            var electedPartitionTask = new WaitElectedPartitionTask(partitions, _electedPartitionNames);
            _electedPartitionTasks.Add(electedPartitionTask);
            return electedPartitionTask.Wait();
        }

        public static EventMeshPartitionedPeerPool Create()
        {
            if (_instance != null) return _instance;
            _instance = new EventMeshPartitionedPeerPool();
            return _instance;
        }

        private static IPeerHost BuildPartitionedNode(int port, ConcurrentBag<ClusterPeer> clusterNodes, int startPeerPort = 30000, Action<string> leaderCallback = null)
        {
            PartitionedNodeHostFactory nodeHostFactory = PartitionedNodeHostFactory.New(options: p =>
            {
                p.Port = port;
            }, nodeOptions: no =>
            {
                no.StartPeerPort = startPeerPort;
                no.CallbackPeerConfiguration = (p) =>
                {
                    p.Services.PostConfigure<RaftConsensusPeerOptions>(o =>
                    {
                        o.LeaderCallback += leaderCallback;
                    });
                };
                no.CallbackPeerDependencies = (s) =>
                {
                    s.AddLogging(l =>
                    {
                        l.ClearProviders();
                        l.AddConsole();
                        l.SetMinimumLevel(LogLevel.Information);
                    });
                };
            }, clusterNodes: clusterNodes)
                .UseUDPTransport()
                .UseEventMesh();
            var node = nodeHostFactory.Build();
            return node;
        }

        private class WaitElectedPartitionTask
        {
            private readonly IEnumerable<string> _partitionNames;
            private readonly ConcurrentBag<string> _electedPartitionNames = null!;
            private bool _isActive = true;
            private readonly SemaphoreSlim _sem = new SemaphoreSlim(0);

            public WaitElectedPartitionTask(IEnumerable<string> partitionNames, ConcurrentBag<string> electedPartitionNames)
            {
                _partitionNames = partitionNames;
                _electedPartitionNames = electedPartitionNames;
            }

            public bool IsActive => _isActive;

            public void ElectPartition(string partitionName)
            {
                if (_electedPartitionNames.Contains(partitionName) || !_partitionNames.Contains(partitionName) || !IsActive) return;
                _electedPartitionNames.Add(partitionName);
                if(_partitionNames.All(p => _electedPartitionNames.Contains(p)))
                {
                    _sem.Release();
                    _isActive = false;
                }
            }

            public Task Wait() => _sem.WaitAsync();
        }
    }
}
