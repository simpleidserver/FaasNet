using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.OpenApi.Results;
using FaasNet.Gateway.Core.Resources;
using FaasNet.StateMachine.Runtime.Exceptions;
using FaasNet.StateMachine.Runtime.OpenAPI;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.OpenApi.Queries.Handlers
{
    public class GetOpenApiOperationQueryHandler : IRequestHandler<GetOpenApiOperationQuery, GetOpenApiApiOperationResult>
    {
        private readonly IOpenAPIParser _openAPIParser;

        public GetOpenApiOperationQueryHandler(IOpenAPIParser openAPIParser)
        {
            _openAPIParser = openAPIParser;
        }

        public async Task<GetOpenApiApiOperationResult> Handle(GetOpenApiOperationQuery request, CancellationToken cancellationToken)
        {
            if (_openAPIParser.TryParseUrl(request.Endpoint, out OpenAPIUrlResult result))
            {
                throw new RequestParameterException(Global.InvalidOpenApiUrl);
            }

            var configuration = await _openAPIParser.GetConfiguration(request.Endpoint, cancellationToken);
            var operations = configuration.Paths.SelectMany(p => p.Value.Select(kvp => kvp.Value));
            var operation = operations.FirstOrDefault(o => o.OperationId == request.OperationId);
            if (operation == null)
            {
                throw new OpenApiOperationNotFoundException(ErrorCodes.UnknownOpenApiOperation, string.Format(Global.UnknownOpenApiOperation, request.OperationId));
            }

            return new GetOpenApiApiOperationResult
            {
                Components = configuration.Components,
                OpenApiOperation = operation
            };
        }
    }
}
