using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.EF.Stores
{
    public class EFApplicationDomainRepository : IApplicationDomainRepository
    {
        private readonly EventMeshDBContext _dbContext;

        public EFApplicationDomainRepository(EventMeshDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(ApplicationDomain applicationDomain)
        {
            _dbContext.ApplicationDomains.Add(applicationDomain);
        }

        public void Delete(ApplicationDomain applicationDomain)
        {
            _dbContext.ApplicationDomains.Remove(applicationDomain);
        }

        public Task<ApplicationDomain> Get(string id, CancellationToken cancellationToken)
        {
            return _dbContext.ApplicationDomains
                .Include(a => a.Applications).ThenInclude(a => a.Links)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ApplicationDomain>> GetAll(string vpn, CancellationToken cancellationToken)
        {
            await EventMeshDBContext.SemaphoreSlim.WaitAsync(cancellationToken);
            var result = await _dbContext.ApplicationDomains
                .Include(a => a.Applications).ThenInclude(a => a.Links)
                .Where(a => a.Vpn == vpn)
                .OrderByDescending(a => a.UpdateDateTime)
                .ToListAsync(cancellationToken);
            EventMeshDBContext.SemaphoreSlim.Release();
            return result;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Update(ApplicationDomain applicationDomain)
        {
            _dbContext.ApplicationDomains.Update(applicationDomain);
        }
    }
}
