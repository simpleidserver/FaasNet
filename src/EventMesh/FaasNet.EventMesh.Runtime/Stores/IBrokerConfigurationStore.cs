using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IBrokerConfigurationStore
    {
        IEnumerable<BrokerConfiguration> GetAll();
        BrokerConfiguration Get(string name);
        void Add(BrokerConfiguration brokerConfiguration);
        int SaveChanges();
    }
}
