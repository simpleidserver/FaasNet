using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    /// <summary>
    /// Update link state.
    /// </summary>
    public class FlowHandler : IRequestHandler
    {
        public string RequestName => "amqp:flow:list";

        public Task<RequestResult> Handle(StateObject state, RequestParameter parameter, CancellationToken cancellationToken)
        {
            return Task.FromResult(RequestResult.Ok());
        }
    }
}
