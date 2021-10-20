using FaasNet.Gateway.Core.ApiDefinitions.Queries.Results;
using FaasNet.Gateway.Core.Common;
using FaasNet.Gateway.Core.Repositories.Parameters;
using MediatR;

namespace FaasNet.Gateway.Core.ApiDefinitions.Queries
{
    public class SearchApiDefinitionsQuery : BaseSearchParameter, IRequest<BaseSearchResult<ApiDefinitionResult>>
    {
    }
}
