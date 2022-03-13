using FaasNet.Application.Core.ApplicationDomain.Queries.Results;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Application.Core.Repositories
{
    public interface IApplicationDomainQueryRepository
    {
        Task<ApplicationDomainResult> Get(string id, CancellationToken cancellationToken);
        void Add(ApplicationDomainResult application);
        void Delete(ApplicationDomainResult application);
        Task<IEnumerable<ApplicationDomainResult>> GetAll(CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
