using FaasNet.Gateway.Core.ApiDefinitions.Queries.Results;
using FaasNet.Gateway.Core.Common;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Repositories.Parameters;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.ApiDefinitions.Queries.Handlers
{
    public class SearchApiDefinitionsQueryHandler : IRequestHandler<SearchApiDefinitionsQuery, BaseSearchResult<ApiDefinitionResult>>
    {
        private readonly IApiDefinitionQueryRepository _apiDefinitionRepository;

        public SearchApiDefinitionsQueryHandler(IApiDefinitionQueryRepository apiDefinitionQueryRepository)
        {
            _apiDefinitionRepository = apiDefinitionQueryRepository;
        }

        public Task<BaseSearchResult<ApiDefinitionResult>> Handle(SearchApiDefinitionsQuery request, CancellationToken cancellationToken)
        {
            return _apiDefinitionRepository.Search(new SearchApisParameter
            {
                Count = request.Count,
                Order = request.Order,
                OrderBy = request.OrderBy,
                StartIndex = request.StartIndex
            }, cancellationToken);
        }
    }
}
