using FaasNet.Gateway.Core.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Repositories
{
    public interface IApiDefinitionRepository
    {
        Task<ApiDefinitionAggregate> GetByPath(string path, CancellationToken cancellationToken);
    }
}
