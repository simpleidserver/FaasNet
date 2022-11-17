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
        private readonly ICollection<WaitElectedPartitionTask> _electedPartitionTasks = new List<WaitElectedPartitionTask>();
        private readonly ConcurrentBag<IPeerHost> _peers = new ConcurrentBag<IPeerHost>();
        private readonly ConcurrentBag<ClusterPeer> _inMemoryCluster = new ConcurrentBag<ClusterPeer>();

        private EventMeshPartitionedPeerPool() { }

        public void AddPartitionedNode(int port, int peerPort)
        {
            var peer = BuildPartitionedNode(port, _inMemoryCluster, peerPort, HandleElectedPartitionName);

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

        public Task WaitAllStandardPartitionsAreLaunched()
        {
            var electedPartitionTask = new WaitElectedPartitionTask(PartitionNames.ALL);
            _electedPartitionTasks.Add(electedPartitionTask);
            return electedPartitionTask.Wait();
        }

        public Task WaitPartition(string partitionName)
        {
            var electedPartitionTask = new WaitElectedPartitionTask(partitionName);
            _electedPartitionTasks.Add(electedPartitionTask);
            return electedPartitionTask.Wait();
        }

        public Task WaitPartitions()
        {

        }

        public static EventMeshPartitionedPeerPool Create() => new EventMeshPartitionedPeerPool();

        private void Check()
        {

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
            private readonly ConcurrentBag<string> _electedPartitionNames = new ConcurrentBag<string>();
            private readonly IEnumerable<string> _partitionNames;
            private bool _isActive = true;
            private readonly SemaphoreSlim _sem = new SemaphoreSlim(0, 1);

            public WaitElectedPartitionTask(string partitionName) => _partitionNames = new[] { partitionName };

            public WaitElectedPartitionTask(IEnumerable<string> partitionNames) => _partitionNames = partitionNames;

            public bool IsActive => _isActive;

            public void ElectPartition(string partitionName)
            {
                if (_electedPartitionNames.Contains(partitionName) || !_partitionNames.Contains(partitionName) || !IsActive) return;
                _electedPartitionNames.Add(partitionName);
                _sem.Release();
                _isActive = false;
            }

            public Task Wait() => _sem.WaitAsync();
        }
    }
}
