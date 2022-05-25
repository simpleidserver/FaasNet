using Amqp.Framing;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    /// <summary>
    /// Indicates that the session has ended.
    /// </summary>
    public class EndHandler : IRequestHandler
    {
        public string RequestName => "amqp:end:list";

        public Task<RequestResult> Handle(StateObject state, RequestParameter parameter, CancellationToken cancellationToken)
        {
            var result = new Frame { Channel = parameter.Channel, Type = FrameTypes.Amqp };
            var close = new End { };
            state.End();
            return Task.FromResult(RequestResult.ExitSession(result.Serialize(close)));
        }
    }
}
