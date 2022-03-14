using FaasNet.EventMesh.Core.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Repositories.InMemory
{
    public class InMemoryEventMeshServerRepository : IEventMeshServerRepository
    {
        private readonly List<EventMeshServerAggregate> _servers;

        public InMemoryEventMeshServerRepository()
        {
            _servers = new List<EventMeshServerAggregate>();
        }

        public Task Add(EventMeshServerAggregate server, CancellationToken cancellationToken)
        {
            _servers.Add(server);
            return Task.CompletedTask;
        }

        public Task<EventMeshServerAggregate> Get(string urn, int port)
        {
            return Task.FromResult(_servers.FirstOrDefault(s => s.Urn == urn && s.Port == port));
        }

        public Task<IEnumerable<EventMeshServerAggregate>> GetAll(CancellationToken cancellationToken)
        {
            return Task.FromResult((IEnumerable<EventMeshServerAggregate>)_servers);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public Task Update(EventMeshServerAggregate server, CancellationToken cancellationToken)
        {
            _servers.Remove(server);
            _servers.Add(server);
            return Task.CompletedTask;
        }
    }
}
