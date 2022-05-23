using Amqp;
using Amqp.Framing;
using Amqp.Types;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public class TransferHandler : IRequestHandler
    {
        private readonly EventMeshAMQPOptions _options;

        public TransferHandler(IOptions<EventMeshAMQPOptions> options)
        {
            _options = options.Value;
        }

        public string RequestName => "amqp:transfer:list";

        public Task<IEnumerable<ByteBuffer>> Handle(StateObject state, DescribedList cmd, byte[] payload, ushort channel, CancellationToken cancellationToken)
        {
            var transfer = cmd as Transfer;
            var disposeResult = new Frame { Channel = channel, Type = FrameTypes.Amqp };
            var dispose = new Dispose
            {
                State = transfer.State
            };
            IEnumerable<ByteBuffer> result = new List<ByteBuffer>
            {
                disposeResult.Serialize(dispose)
            };
            return Task.FromResult(result);
        }

        private async Task PublishMessage(StateObject state, byte[] payload)
        {
            // state.Session.EventMeshPubSession.Publish()
        }
    }
}
