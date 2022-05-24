using Amqp;
using Amqp.Framing;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public class BeginHandler : IRequestHandler
    {
        public string RequestName => "amqp:begin:list";

        public Task<IEnumerable<ByteBuffer>> Handle(StateObject state, RequestParameter parameter, CancellationToken cancellationToken)
        {
            var beginResult = new Frame { Channel = parameter.Channel, Type = FrameTypes.Amqp };
            var begin = new Begin { RemoteChannel = parameter.Channel };
            IEnumerable<ByteBuffer> result = new[]
            {
                beginResult.Serialize(begin)
            };
            return Task.FromResult(result);
        }
    }
}
