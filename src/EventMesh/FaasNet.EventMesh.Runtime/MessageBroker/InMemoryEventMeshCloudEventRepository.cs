using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public interface IEventMeshCloudEventRepository
    {
        void Add(EventMeshCloudEvent evtMeshCloudEvt);
        Task<int> SaveChanges(CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<EventMeshCloudEvent>> GetLatest(List<TopicExpression> filters, int offset, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class InMemoryEventMeshCloudEventRepository : IEventMeshCloudEventRepository
    {
        private readonly ConcurrentBag<EventMeshCloudEvent> _cloudEvts;

        public InMemoryEventMeshCloudEventRepository()
        {
            _cloudEvts = new ConcurrentBag<EventMeshCloudEvent>();
        }

        public void Add(EventMeshCloudEvent evtMeshCloudEvt)
        {
            _cloudEvts.Add(evtMeshCloudEvt);
        }

        public Task<IEnumerable<EventMeshCloudEvent>> GetLatest(List<TopicExpression> filters, int offset, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<EventMeshCloudEvent> filteredCloudEvts = _cloudEvts.AsQueryable().OrderBy(c => c.CreateDateTime).Query(filters).Skip(offset);
            return Task.FromResult(filteredCloudEvts);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(1);
        }
    }
}
