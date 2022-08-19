using FaasNet.Partition.Client.Messages;
using FaasNet.Peer;
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
        private readonly ICollection<(IPeerHost, DirectPartitionPeer)> _partitions;

        public DirectPartitionCluster(IPartitionPeerFactory partitionPeerFactory, IPartitionPeerStore partitionPeerStore)
        {
            _partitionPeerFactory = partitionPeerFactory;
            _partitionPeerStore = partitionPeerStore;
            _partitions = new List<(IPeerHost, DirectPartitionPeer)>();
        }

        public async Task Start()
        {
            var partitions = await _partitionPeerStore.GetAll();
            foreach (var partition in partitions)
            {
                var peerHost = _partitionPeerFactory.Build(partition.Port, partition.PartitionKey);
                _partitions.Add((peerHost, partition));
                await peerHost.Start();
            }
        }

        public async Task Stop()
        {
            foreach (var partition in _partitions)
                await partition.Item1.Stop();
        }

        public async Task<byte[]> Transfer(TransferedRequest request, CancellationToken cancellationToken)
        {
            var peer = _partitions.First(p => p.Item2.PartitionKey == request.PartitionKey);
            using(var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0)))
            {
                var target = new IPEndPoint(IPAddress.Loopback, peer.Item2.Port);
                await udpClient.SendAsync(request.Content, target, cancellationToken);
                var receivedResult = await udpClient.ReceiveAsync(cancellationToken);
                return receivedResult.Buffer;
            }
        }
    }

    public class DirectPartitionPeer
    {
        public int Port { get; set; }
        public string PartitionKey { get; set; }
    }
}
