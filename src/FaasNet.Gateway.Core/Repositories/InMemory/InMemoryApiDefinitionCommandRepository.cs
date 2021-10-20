using FaasNet.Gateway.Core.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Repositories.InMemory
{
    public class InMemoryApiDefinitionCommandRepository : IApiDefinitionCommandRepository
    {
        private ICollection<ApiDefinitionAggregate> _apiDefinitions;

        public InMemoryApiDefinitionCommandRepository(ICollection<ApiDefinitionAggregate> apiDefinitions)
        {
            _apiDefinitions = apiDefinitions;
        }

        public Task AddOrUpdate(ApiDefinitionAggregate apiDef, CancellationToken cancellationToken)
        {
            var record = _apiDefinitions.FirstOrDefault(a => a.Name == apiDef.Name);
            if (record != null)
            {
                _apiDefinitions.Remove(record);
            }

            _apiDefinitions.Add((ApiDefinitionAggregate)apiDef.Clone());
            return Task.CompletedTask;
        }

        public Task<ApiDefinitionAggregate> GetByName(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(_apiDefinitions.FirstOrDefault(d => d.Name == name));
        }

        public Task<ApiDefinitionAggregate> GetByPath(string fullPath, CancellationToken cancellationToken)
        {
            return Task.FromResult(_apiDefinitions.FirstOrDefault(a => fullPath.StartsWith(a.Path)));
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
