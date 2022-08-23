using FaasNet.Peer;
using FaasNet.Peer.Client.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public class DirectPartitionCluster : IPartitionCluster
    {
        private readonly IPartitionPeerFactory _partitionPeerFactory;
        private readonly IPartitionPeerStore _partitionPeerStore;
        private readonly PartitionedNodeOptions _options;
        private readonly ICollection<(IPeerHost, DirectPartitionPeer)> _partitions;

        public DirectPartitionCluster(IPartitionPeerFactory partitionPeerFactory, IPartitionPeerStore partitionPeerStore, IOptions<PartitionedNodeOptions> options)
        {
            _partitionPeerFactory = partitionPeerFactory;
            _partitionPeerStore = partitionPeerStore;
            _partitions = new List<(IPeerHost, DirectPartitionPeer)>();
            _options = options.Value;
        }

        public async Task Start()
        {
            var partitions = await _partitionPeerStore.GetAll();
            foreach (var partition in partitions)
            {
                var peerHost = _partitionPeerFactory.Build(partition.Port, partition.PartitionKey, _options.PeerConfiguration);
                _partitions.Add((peerHost, partition));
                await peerHost.Start();
            }
        }

        public async Task Stop()
        {
            foreach (var partition in _partitions)
                await partition.Item1.Stop();
        }

        public async Task AddAndStart(string partitionKey)
        {
            var port = !_partitions.Any() ? _options.StartPeerPort : _partitions.OrderByDescending(p => p.Item2.Port).First().Item2.Port + 1;
            var record = new DirectPartitionPeer { PartitionKey = partitionKey, Port = port };
            await _partitionPeerStore.Add(record);
            var peerHost = _partitionPeerFactory.Build(port, partitionKey, _options.PeerConfiguration);
            await peerHost.Start();
            _partitions.Add((peerHost, record));
        }

        public async Task<byte[]> Transfer(TransferedRequest request, CancellationToken cancellationToken)
        {
            var peer = _partitions.First(p => p.Item2.PartitionKey == request.PartitionKey);
            using(var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0))) // Move ??
            {
                var target = new IPEndPoint(IPAddress.Loopback, peer.Item2.Port);
                await udpClient.SendAsync(request.Content, target, cancellationToken);
                var receivedResult = await udpClient.ReceiveAsync(cancellationToken);
                return receivedResult.Buffer;
            }
        }

        public async Task<IEnumerable<byte[]>> Broadcast(BroadcastRequest request, CancellationToken cancellationToken)
        {
            var result = new ConcurrentBag<byte[]>();
            await Parallel.ForEachAsync(_partitions, new ParallelOptions
            {
                MaxDegreeOfParallelism = _options.MaxConcurrentThreads
            }, async (peer, t) =>
            {
                using (var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
                {
                    var target = new IPEndPoint(IPAddress.Loopback, peer.Item2.Port);
                    await udpClient.SendAsync(request.Content, target, cancellationToken);
                    var receivedResult = await udpClient.ReceiveAsync(cancellationToken);
                    result.Add(receivedResult.Buffer);
                }
            });

            return result.ToArray();
        }
    }

    public class DirectPartitionPeer
    {
        public int Port { get; set; }
        public string PartitionKey { get; set; }
    }
}
