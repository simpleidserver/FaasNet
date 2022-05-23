using Amqp;
using Amqp.Framing;
using Amqp.Types;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public class BeginHandler : IRequestHandler
    {
        public string RequestName => "amqp:begin:list";

        public Task<IEnumerable<ByteBuffer>> Handle(StateObject state, DescribedList cmd, byte[] payload, ushort channel, CancellationToken cancellationToken)
        {
            var requestBegin = cmd as Begin;
            var beginResult = new Frame { Channel = channel, Type = FrameTypes.Amqp };
            var begin = new Begin { RemoteChannel = channel };
            IEnumerable<ByteBuffer> result = new[]
            {
                beginResult.Serialize(begin)
            };
            return Task.FromResult(result);
        }
    }
}
