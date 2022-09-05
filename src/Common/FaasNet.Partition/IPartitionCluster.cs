using FaasNet.Peer.Client.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public interface IPartitionCluster
    {
        Task Start();
        Task Stop();
        Task<bool> Contains(string partitionKey, CancellationToken cancellationToken);
        Task<bool> TryAddAndStart(string partitionKey, Type stateMachineType = null);
        Task<byte[]> Transfer(TransferedRequest request, CancellationToken cancellationToken);
        Task<IEnumerable<byte[]>> Broadcast(BroadcastRequest request, CancellationToken cancellationToken);
    }
}
