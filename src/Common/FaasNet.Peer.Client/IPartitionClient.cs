using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Client
{
    public interface IPartitionClient : IDisposable
    {
        Task AddPartition(string partitionKey, CancellationToken cancellationToken);
    }
}
