using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public class DirectPartitionCluster : IPartitionCluster
    {
        private readonly IPartitionPeerFactory _partitionPeerFactory;
        private readonly IPartitionPeerStore _partitionPeerStore;
        private readonly IClientTransportFactory _clientTransportFactory;
        private readonly PartitionedNodeOptions _options;
        private readonly ICollection<(IPeerHost, DirectPartitionPeer)> _partitions;

        public DirectPartitionCluster(IPartitionPeerFactory partitionPeerFactory, IPartitionPeerStore partitionPeerStore, IClientTransportFactory clientTransportFactory, IOptions<PartitionedNodeOptions> options)
        {
            _partitionPeerFactory = partitionPeerFactory;
            _partitionPeerStore = partitionPeerStore;
            _clientTransportFactory = clientTransportFactory;
            _options = options.Value;
            _partitions = new List<(IPeerHost, DirectPartitionPeer)>();
        }

        public async Task Start()
        {
            var partitions = await _partitionPeerStore.GetAll();
            foreach (var partition in partitions)
            {
                var peerHost = _partitionPeerFactory.Build(partition.Port, partition.PartitionKey, partition.StateMachineType, _options.CallbackPeerDependencies, _options.CallbackPeerConfiguration);
                _partitions.Add((peerHost, partition));
                await peerHost.Start();
            }
        }

        public async Task Stop()
        {
            foreach (var partition in _partitions)
                await partition.Item1.Stop();
        }

        public async Task<bool> Contains(string partitionKey, CancellationToken cancellationToken)
        {
            var partitions = await _partitionPeerStore.GetAll();
            return partitions.Any(p => p.PartitionKey == partitionKey);
        }

        public async Task<bool> TryAddAndStart(string partitionKey, Type stateMachineType = null)
        {
            var partition = await _partitionPeerStore.Get(partitionKey);
            if (partition != null) return false;
            var port = !_partitions.Any() ? _options.StartPeerPort : _partitions.OrderByDescending(p => p.Item2.Port).First().Item2.Port + 1;
            var record = new DirectPartitionPeer { PartitionKey = partitionKey, Port = port, StateMachineType = stateMachineType };
            await _partitionPeerStore.Add(record);
            var peerHost = _partitionPeerFactory.Build(port, partitionKey, stateMachineType, _options.CallbackPeerDependencies, _options.CallbackPeerConfiguration);
            await peerHost.Start();
            _partitions.Add((peerHost, record));
            return true;
        }

        public async Task<byte[]> Transfer(TransferedRequest request, CancellationToken cancellationToken)
        {
            var peer = _partitions.First(p => p.Item2.PartitionKey == request.PartitionKey);
            using (var transport = _clientTransportFactory.Create())
            {
                transport.Open(new IPEndPoint(IPAddress.Loopback, peer.Item2.Port));
                await transport.Send(request.Content, cancellationToken: cancellationToken);
                var receivedResult = await transport.Receive(cancellationToken: cancellationToken);
                return receivedResult;
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
                using (var transport = _clientTransportFactory.Create())
                {
                    transport.Open(new IPEndPoint(IPAddress.Loopback, peer.Item2.Port));
                    await transport.Send(request.Content, 10000, cancellationToken: cancellationToken);
                    var receivedResult = await transport.Receive(10000, cancellationToken: cancellationToken);
                    result.Add(receivedResult);
                }
            });

            return result.ToArray();
        }
    }

    public class DirectPartitionPeer
    {
        public int Port { get; set; }
        public string PartitionKey { get; set; }
        public Type StateMachineType { get; set; }
    }
}
