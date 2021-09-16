using FaasNet.Gateway.Core.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Repositories.InMemory
{
    public class InMemoryApiDefinitionRepository : IApiDefinitionRepository
    {
        private ICollection<ApiDefinitionAggregate> _apiDefinitions;

        public InMemoryApiDefinitionRepository(ICollection<ApiDefinitionAggregate> apiDefinitions)
        {
            _apiDefinitions = apiDefinitions;
        }

        public Task<ApiDefinitionAggregate> GetByPath(string fullPath, CancellationToken cancellationToken)
        {
            return Task.FromResult(_apiDefinitions.FirstOrDefault(a => fullPath.StartsWith(a.Path)));
        }
    }
}
