using FaasNet.Gateway.Core.Common;
using FaasNet.Gateway.Core.Functions.Queries.Results;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Repositories.Parameters;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Queries.Handlers
{
    public class SearchFunctionsQueryHandler : IRequestHandler<SearchFunctionsQuery, BaseSearchResult<FunctionResult>>
    {
        private readonly IFunctionQueryRepository _functionRepository;

        public SearchFunctionsQueryHandler(IFunctionQueryRepository functionRepository)
        {
            _functionRepository = functionRepository;
        }

        public Task<BaseSearchResult<FunctionResult>> Handle(SearchFunctionsQuery request, CancellationToken cancellationToken)
        {
            return _functionRepository.Search(new SearchFunctionsParameter
            {
                Count = request.Count,
                Order = request.Order,
                OrderBy = request.OrderBy,
                StartIndex = request.StartIndex
            }, cancellationToken);
        }
    }
}
