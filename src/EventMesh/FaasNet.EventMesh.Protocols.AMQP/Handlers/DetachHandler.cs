using Amqp.Framing;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    /// <summary>
    /// Detach the link endpoint from the session.
    /// </summary>
    public class DetachHandler : IRequestHandler
    {
        public string RequestName => "amqp:detach:list";

        public Task<RequestResult> Handle(StateObject state, RequestParameter parameter, CancellationToken cancellationToken)
        {
            var detachCmd = parameter.Cmd as Detach;
            var frame = new Frame { Channel = parameter.Channel, Type = FrameTypes.Amqp };
            var cmd = new Detach
            {
                Handle = detachCmd.Handle,
                Error = null,
                Closed = true
            };
            var result = frame.Serialize(cmd);
            return Task.FromResult(RequestResult.ExitSession(result));
        }
    }
}
