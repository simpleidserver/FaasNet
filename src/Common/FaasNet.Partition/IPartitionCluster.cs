using FaasNet.Partition.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public interface IPartitionCluster
    {
        Task Start();
        Task Stop();
        Task<byte[]> Transfer(TransferedRequest request, CancellationToken cancellationToken);
    }
}
