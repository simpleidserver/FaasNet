using FaasNet.Gateway.Core.ApiDefinitions.Queries.Results;
using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Repositories;
using FaasNet.Gateway.Core.Resources;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.ApiDefinitions.Queries.Handlers
{
    public class GetApiDefinitionQueryHandler : IRequestHandler<GetApiDefinitionQuery, ApiDefinitionResult>
    {
        private readonly IApiDefinitionQueryRepository _apiDefinitionQueryRepository;

        public GetApiDefinitionQueryHandler(IApiDefinitionQueryRepository apiDefinitionQueryRepository)
        {
            _apiDefinitionQueryRepository = apiDefinitionQueryRepository;
        }

        public async Task<ApiDefinitionResult> Handle(GetApiDefinitionQuery request, CancellationToken cancellationToken)
        {
            var funcResult = await _apiDefinitionQueryRepository.Get(request.Name, cancellationToken);
            if (funcResult == null)
            {
                throw new ApiDefNotFoundException(string.Format(Global.UnknownApiDef, request.Name));
            }

            return funcResult;
        }
    }
}
