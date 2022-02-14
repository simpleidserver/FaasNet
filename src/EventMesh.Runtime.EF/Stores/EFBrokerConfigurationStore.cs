using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EventMesh.Runtime.EF.Stores
{
    public class EFBrokerConfigurationStore : IBrokerConfigurationStore
    {
        private readonly EventMeshDBContext _dbContext;

        public EFBrokerConfigurationStore(EventMeshDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<BrokerConfiguration> GetAll()
        {
            return _dbContext.BrokerConfigurations.Include(b => b.Records).ToList();
        }

        public BrokerConfiguration Get(string name)
        {
            return _dbContext.BrokerConfigurations.Include(b => b.Records).FirstOrDefault(b => b.Name == name);
        }

        public void Add(BrokerConfiguration brokerConfiguration)
        {
            _dbContext.BrokerConfigurations.Add(brokerConfiguration);
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}
