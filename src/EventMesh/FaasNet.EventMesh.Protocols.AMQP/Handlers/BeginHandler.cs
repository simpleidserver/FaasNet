using Amqp.Framing;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public class BeginHandler : IRequestHandler
    {
        public string RequestName => "amqp:begin:list";

        public Task<RequestResult> Handle(StateObject state, RequestParameter parameter, CancellationToken cancellationToken)
        {
            var beginResult = new Frame { Channel = parameter.Channel, Type = FrameTypes.Amqp };
            var begin = new Begin { RemoteChannel = parameter.Channel };
            return Task.FromResult(RequestResult.Ok(beginResult.Serialize(begin)));
        }
    }
}
