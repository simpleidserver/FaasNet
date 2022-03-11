using FaasNet.Application.Core.ApplicationDomain.Queries.Results;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Application.Core.Repositories
{
    public interface IApplicationDomainQueryRepository
    {
        void Add(ApplicationDomainResult application);
        Task<IEnumerable<ApplicationDomainResult>> GetAll(CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
