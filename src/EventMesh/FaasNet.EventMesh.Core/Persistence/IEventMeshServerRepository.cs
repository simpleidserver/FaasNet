using FaasNet.EventMesh.Core.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Repositories
{
    public interface IEventMeshServerRepository
    {
        Task<EventMeshServerAggregate> Get(string urn, int port);
        Task Add(EventMeshServerAggregate server, CancellationToken cancellationToken);
        Task Update(EventMeshServerAggregate server, CancellationToken cancellationToken);
        Task<IEnumerable<EventMeshServerAggregate>> GetAll(CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
