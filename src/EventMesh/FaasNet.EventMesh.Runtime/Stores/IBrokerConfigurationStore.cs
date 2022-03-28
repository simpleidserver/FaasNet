using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IBrokerConfigurationStore
    {
        Task<IEnumerable<BrokerConfiguration>> GetAll(CancellationToken cancellationToken);
        Task<BrokerConfiguration> Get(string name, CancellationToken cancellationToken);
        void Add(BrokerConfiguration brokerConfiguration);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
