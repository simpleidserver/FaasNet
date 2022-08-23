using FaasNet.Peer.Client.Messages;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public interface IPartitionCluster
    {
        Task Start();
        Task Stop();
        Task AddAndStart(string partitionKey);
        Task<byte[]> Transfer(TransferedRequest request, CancellationToken cancellationToken);
        Task<IEnumerable<byte[]>> Broadcast(BroadcastRequest request, CancellationToken cancellationToken);
    }
}
