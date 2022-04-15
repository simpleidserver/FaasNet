using FaasNet.EventMesh.Runtime.MessageBroker;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.EF.Stores
{
    public class EFEventMeshCloudEventRepository : IEventMeshCloudEventRepository
    {
        private readonly MessageBrokerDBContext _dbContext;

        public EFEventMeshCloudEventRepository(MessageBrokerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(EventMeshCloudEvent evtMeshCloudEvt)
        {
            _dbContext.CloudEvts.Add(evtMeshCloudEvt);
        }

        public async Task<IEnumerable<EventMeshCloudEvent>> GetLatest(List<TopicExpression> filters, int offset, CancellationToken cancellationToken = default)
        {
            IEnumerable<EventMeshCloudEvent> filteredCloudEvts = await _dbContext.CloudEvts.OrderBy(c => c.CreateDateTime).Query(filters).Skip(offset).ToListAsync(cancellationToken);
            return filteredCloudEvts;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
