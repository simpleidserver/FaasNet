using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IApplicationDomainRepository
    {
        Task<ApplicationDomain> Get(string id, CancellationToken cancellationToken);
        void Add(ApplicationDomain applicationDomain);
        void Delete(ApplicationDomain applicationDomain);
        void Update(ApplicationDomain applicationDomain);
        Task<int> SaveChanges(CancellationToken cancellationToken);
        Task<IEnumerable<ApplicationDomain>> GetAll(string vpn, CancellationToken cancellationToken);
    }
}
