using FaasNet.Gateway.Core.Common;
using FaasNet.Gateway.Core.Functions.Queries.Results;
using FaasNet.Gateway.Core.Repositories.Parameters;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Repositories
{
    public interface IFunctionQueryRepository
    {
        Task<BaseSearchResult<FunctionResult>> Search(SearchFunctionsParameter parameter, CancellationToken cancellationToken);
        Task<FunctionResult> Get(string name, CancellationToken cancellationToken);
    }
}
