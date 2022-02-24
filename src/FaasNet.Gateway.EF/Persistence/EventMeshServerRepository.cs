using FaasNet.Gateway.Core.Domains;
using FaasNet.Gateway.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.EF.Persistence
{
    public class EventMeshServerRepository : IEventMeshServerRepository
    {
        private readonly GatewayDBContext _gatewayDBContext;

        public EventMeshServerRepository(GatewayDBContext gatewayDBContext)
        {
            _gatewayDBContext = gatewayDBContext;
        }

        public Task Add(EventMeshServerAggregate server, CancellationToken cancellationToken)
        {
            _gatewayDBContext.EventMeshServers.Add(server);
            return Task.CompletedTask;
        }

        public Task<EventMeshServerAggregate> Get(string urn, int port)
        {
            return _gatewayDBContext.EventMeshServers.Include(e => e.Bridges).FirstOrDefaultAsync(e => e.Urn == urn && e.Port == port);
        }

        public async Task<IEnumerable<EventMeshServerAggregate>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _gatewayDBContext.EventMeshServers.Include(e => e.Bridges).ToListAsync(cancellationToken);
            return result;
        }

        public Task Update(EventMeshServerAggregate server, CancellationToken cancellationToken)
        {
            _gatewayDBContext.EventMeshServers.Update(server);
            return Task.CompletedTask;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _gatewayDBContext.SaveChangesAsync(cancellationToken);
        }
    }
}
