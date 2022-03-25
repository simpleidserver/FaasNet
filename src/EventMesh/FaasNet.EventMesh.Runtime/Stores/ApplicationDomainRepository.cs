using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public class ApplicationDomainRepository : IApplicationDomainRepository
    {
        private readonly ICollection<ApplicationDomain> _applicationDomains;

        public ApplicationDomainRepository(ICollection<ApplicationDomain> applicationDomains)
        {
            _applicationDomains = applicationDomains;
        }

        public void Add(ApplicationDomain applicationDomain)
        {
            _applicationDomains.Add(applicationDomain);
        }

        public void Delete(ApplicationDomain applicationDomain)
        {
            _applicationDomains.Remove(applicationDomain);
        }

        public Task<ApplicationDomain> Get(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_applicationDomains.FirstOrDefault(a => a.Id == id));
        }

        public Task<IEnumerable<ApplicationDomain>> GetAll(string vpn, CancellationToken cancellationToken)
        {
            IEnumerable<ApplicationDomain> result = _applicationDomains;
            return Task.FromResult(result);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public void Update(ApplicationDomain applicationDomain)
        {
            _applicationDomains.Remove(applicationDomain);
            _applicationDomains.Add(applicationDomain);
        }
    }
}
