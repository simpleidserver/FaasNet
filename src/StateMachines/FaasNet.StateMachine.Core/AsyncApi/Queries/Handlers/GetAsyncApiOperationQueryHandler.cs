using FaasNet.Domain.Exceptions;
using FaasNet.StateMachine.Core.OpenApi.Results;
using FaasNet.StateMachine.Core.Resources;
using FaasNet.StateMachine.Runtime.AsyncAPI;
using FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.AsyncApi.Queries.Handlers
{
    public class GetAsyncApiOperationQueryHandler : IRequestHandler<GetAsyncApiOperationQuery, GetAsyncApiOperationResult>
    {
        private readonly IAsyncAPIParser _asyncApiParser;

        public GetAsyncApiOperationQueryHandler(IAsyncAPIParser asyncApiParser)
        {
            _asyncApiParser = asyncApiParser;
        }

        public async Task<GetAsyncApiOperationResult> Handle(GetAsyncApiOperationQuery request, CancellationToken cancellationToken)
        {
            if (_asyncApiParser.TryParseUrl(request.Endpoint, out AsyncApiResult result))
            {
                throw new BadRequestException(ErrorCodes.INVALID_ASYNCAPI_URL, Global.InvalidAsyncApiUrl);
            }

            var configuration = await _asyncApiParser.GetConfiguration(request.Endpoint, cancellationToken);
            var operations = configuration.Channels.Select(c => c.Value.Publish);
            var operation = operations.FirstOrDefault(o => o.OperationId == request.OperationId);
            if (operation == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_ASYNCAPI_OPERATION, string.Format(Global.UnknownAsyncApiOperation, request.OperationId));
            }

            ProcessReference(operation);
            return new GetAsyncApiOperationResult
            {
                Components = configuration.Components,
                AsyncApiOperation = operation
            };
        }

        private void ProcessReference(Operation operation)
        {
            var reference = operation.Message.Reference;
            if (reference != null)
            {
                operation.Message = reference;
            }

            var binding = operation.Bindings.Reference;
            if(binding != null)
            {
                operation.Bindings = binding;
            }
        }
    }
}
