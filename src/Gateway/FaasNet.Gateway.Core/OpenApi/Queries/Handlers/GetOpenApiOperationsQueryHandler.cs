using FaasNet.Gateway.Core.Resources;
using FaasNet.StateMachine.Runtime.Exceptions;
using FaasNet.StateMachine.Runtime.OpenAPI;
using FaasNet.StateMachine.Runtime.OpenAPI.v3.Models;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.OpenApi.Queries.Handlers
{
    public class GetOpenApiOperationsQueryHandler : IRequestHandler<GetOpenApiOperationsQuery, IEnumerable<OpenApiOperationResult>>
    {
        private readonly IOpenAPIParser _openAPIParser;

        public GetOpenApiOperationsQueryHandler(IOpenAPIParser openAPIParser)
        {
            _openAPIParser = openAPIParser;
        }

        public async Task<IEnumerable<OpenApiOperationResult>> Handle(GetOpenApiOperationsQuery request, CancellationToken cancellationToken)
        {
            if (_openAPIParser.TryParseUrl(request.Endpoint, out OpenAPIUrlResult result))
            {
                throw new RequestParameterException(Global.InvalidOpenApiUrl);
            }

            var configuration = await _openAPIParser.GetConfiguration(request.Endpoint, cancellationToken);
            var operations = configuration.Paths.SelectMany(p => p.Value.Select(kvp => kvp.Value));
            return operations;
        }
    }
}