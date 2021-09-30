using FaasNet.Gateway.Core.Common;
using FaasNet.Gateway.Core.Functions.Queries.Results;
using FaasNet.Gateway.Core.Repositories.Parameters;
using MediatR;

namespace FaasNet.Gateway.Core.Functions.Queries
{
    public class SearchFunctionsQuery : BaseSearchParameter, IRequest<BaseSearchResult<FunctionResult>>
    {
    }
}
