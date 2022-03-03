using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Linq;

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

        public BrokerConfiguration Get(string name)
        {
            return _brokerConfigurations.FirstOrDefault(s => s.Name == name);
        }

        public IEnumerable<BrokerConfiguration> GetAll()
        {
            return _brokerConfigurations;
        }

        public int SaveChanges()
        {
            return 1;
        }
    }
}
