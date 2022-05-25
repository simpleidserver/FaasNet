using Amqp.Framing;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    /// <summary>
    /// Sender will not be sending any more frames on the connection.
    /// </summary>
    public class CloseHandler : IRequestHandler
    {
        public string RequestName => "amqp:close:list";

        public Task<RequestResult> Handle(StateObject state, RequestParameter parameter, CancellationToken cancellationToken)
        {
            var result = new Frame { Channel = parameter.Channel, Type = FrameTypes.Amqp };
            var close = new Close { };
            state.End();
            return Task.FromResult(RequestResult.ExitConnection(result.Serialize(close)));
        }
    }
}
