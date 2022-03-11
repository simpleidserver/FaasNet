using FaasNet.Application.Core.ApplicationDomain.Queries.Results;
using FaasNet.Application.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Application.EF.Repositories
{
    public class EFApplicationDomainQueryRepository : IApplicationDomainQueryRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public EFApplicationDomainQueryRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(ApplicationDomainResult applicationDomain)
        {
            _dbContext.ApplicationDomains.Add(applicationDomain);
        }

        public async Task<IEnumerable<ApplicationDomainResult>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<ApplicationDomainResult> result = await _dbContext.ApplicationDomains.ToListAsync(cancellationToken);
            return result;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
