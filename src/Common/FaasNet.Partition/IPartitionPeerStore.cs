using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public interface IPartitionPeerStore
    {
        Task<IEnumerable<DirectPartitionPeer>> GetAll();
    }

    public class InMemoryPartitionPeerStore : IPartitionPeerStore
    {
        private readonly IEnumerable<DirectPartitionPeer> _partitions;

        public InMemoryPartitionPeerStore(IEnumerable<DirectPartitionPeer> partitions)
        {
            _partitions = partitions;
        }

        public Task<IEnumerable<DirectPartitionPeer>> GetAll()
        {
            return Task.FromResult(_partitions);
        }
    }
}
