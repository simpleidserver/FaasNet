using Amqp;
using Amqp.Types;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public class OpenHandler : IRequestHandler
    {
        public string RequestName => "amqp:open:list";

        public Task<IEnumerable<ByteBuffer>> Handle(StateObject state, DescribedList cmd, byte[] payload, ushort channel, CancellationToken cancellationToken)
        {
            var openResult = new Frame { Channel = channel, Type = FrameTypes.Amqp };
            var cmdResult = new Amqp.Framing.Open
            {
                HostName = "localhost"
            };
            IEnumerable<ByteBuffer> result = new[]
            {
                openResult.Serialize(cmdResult)
            };
            return Task.FromResult(result);
        }
    }
}
