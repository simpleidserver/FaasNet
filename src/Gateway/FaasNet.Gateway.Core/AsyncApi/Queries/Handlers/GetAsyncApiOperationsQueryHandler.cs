using FaasNet.Gateway.Core.Resources;
using FaasNet.StateMachine.Runtime.AsyncAPI;
using FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models;
using FaasNet.StateMachine.Runtime.Exceptions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.AsyncApi.Queries.Handlers
{
    public class GetAsyncApiOperationsQueryHandler : IRequestHandler<GetAsyncApiOperationsQuery, IEnumerable<Operation>>
    {
        private readonly IAsyncAPIParser _asyncAPIParser;

        public GetAsyncApiOperationsQueryHandler(IAsyncAPIParser asyncAPIParser)
        {
            _asyncAPIParser = asyncAPIParser;
        }

        public async Task<IEnumerable<Operation>> Handle(GetAsyncApiOperationsQuery request, CancellationToken cancellationToken)
        {
            if (_asyncAPIParser.TryParseUrl(request.Endpoint, out AsyncApiResult result))
            {
                throw new RequestParameterException(Global.InvalidAsyncApiUrl);
            }

            var configuration = await _asyncAPIParser.GetConfiguration(request.Endpoint, cancellationToken);
            var operations = configuration.Channels.Select(c => c.Value.Publish);
            return operations;
        }
    }
}