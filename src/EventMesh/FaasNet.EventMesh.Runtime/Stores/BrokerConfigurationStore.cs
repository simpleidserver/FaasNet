using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public class BrokerConfigurationStore : IBrokerConfigurationStore
    {
        private readonly ICollection<BrokerConfiguration> _brokerConfigurations;

        public BrokerConfigurationStore()
        {
            _brokerConfigurations = new List<BrokerConfiguration>();
        }

        public void Add(BrokerConfiguration brokerConfiguration)
        {
            _brokerConfigurations.Add(brokerConfiguration);
        }

        public Task<BrokerConfiguration> Get(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(_brokerConfigurations.FirstOrDefault(s => s.Name == name));
        }

        public Task<IEnumerable<BrokerConfiguration>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<BrokerConfiguration> result = _brokerConfigurations;
            return Task.FromResult(result);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
