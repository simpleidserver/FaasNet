using FaasNet.EventMesh.Core.Domains;
using FaasNet.EventMesh.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.EF
{
    public class EFEventMeshServerRepository : IEventMeshServerRepository
    {
        private readonly EventMeshDBContext _dbContext;

        public EFEventMeshServerRepository(EventMeshDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Add(EventMeshServerAggregate server, CancellationToken cancellationToken)
        {
            _dbContext.EventMeshServers.Add(server);
            return Task.CompletedTask;
        }

        public Task<EventMeshServerAggregate> Get(string urn, int port)
        {
            return _dbContext.EventMeshServers.FirstOrDefaultAsync(e => e.Urn == urn && e.Port == port);
        }

        public async Task<IEnumerable<EventMeshServerAggregate>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<EventMeshServerAggregate> result = await _dbContext.EventMeshServers.ToListAsync(cancellationToken);
            return result;
        }

        public Task Update(EventMeshServerAggregate server, CancellationToken cancellationToken)
        {
            _dbContext.EventMeshServers.Update(server);
            return Task.CompletedTask;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
