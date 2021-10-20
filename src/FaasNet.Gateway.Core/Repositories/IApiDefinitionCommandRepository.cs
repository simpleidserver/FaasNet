using FaasNet.Gateway.Core.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Repositories
{
    public interface IApiDefinitionCommandRepository
    {
        Task<ApiDefinitionAggregate> GetByName(string name, CancellationToken cancellationToken);
        Task<ApiDefinitionAggregate> GetByPath(string path, CancellationToken cancellationToken);
        Task AddOrUpdate(ApiDefinitionAggregate apiDef, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
