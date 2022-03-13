using FaasNet.Domain;
using FaasNet.Function.Core.Functions.Queries.Results;
using MediatR;

namespace FaasNet.Function.Core.Functions.Queries
{
    public class SearchFunctionsQuery : BaseSearchParameter, IRequest<BaseSearchResult<FunctionResult>>
    {
    }
}
