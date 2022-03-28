using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.EF.Stores
{
    public class EFBrokerConfigurationStore : IBrokerConfigurationStore
    {
        private readonly EventMeshDBContext _dbContext;

        public EFBrokerConfigurationStore(EventMeshDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<BrokerConfiguration>> GetAll(CancellationToken cancellationToken)
        {
            await EventMeshDBContext.SemaphoreSlim.WaitAsync(cancellationToken);
            var result = await _dbContext.BrokerConfigurations.Include(b => b.Records).ToListAsync(cancellationToken);
            EventMeshDBContext.SemaphoreSlim.Release();
            return result;
        }

        public async Task<BrokerConfiguration> Get(string name, CancellationToken cancellationToken)
        {
            await EventMeshDBContext.SemaphoreSlim.WaitAsync(cancellationToken);
            var result = await _dbContext.BrokerConfigurations.Include(b => b.Records).FirstOrDefaultAsync(b => b.Name == name);
            EventMeshDBContext.SemaphoreSlim.Release();
            return result;
        }

        public void Add(BrokerConfiguration brokerConfiguration)
        {
            _dbContext.BrokerConfigurations.Add(brokerConfiguration);
        }

        public async Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            await EventMeshDBContext.SemaphoreSlim.WaitAsync(cancellationToken);
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            EventMeshDBContext.SemaphoreSlim.Release();
            return result;
        }
    }
}
