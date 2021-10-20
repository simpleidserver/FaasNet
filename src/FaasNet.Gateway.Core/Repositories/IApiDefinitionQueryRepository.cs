using FaasNet.Gateway.Core.ApiDefinitions.Queries.Results;
using FaasNet.Gateway.Core.Common;
using FaasNet.Gateway.Core.Repositories.Parameters;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Repositories
{
    public interface IApiDefinitionQueryRepository
    {
        Task<BaseSearchResult<ApiDefinitionResult>> Search(SearchApisParameter parameter, CancellationToken cancellationToken);
        Task<ApiDefinitionResult> Get(string name, CancellationToken cancellationToken);
    }
}
