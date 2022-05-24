using Amqp;
using Amqp.Framing;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public class TransferHandler : IRequestHandler
    {
        public string RequestName => "amqp:transfer:list";

        public async Task<IEnumerable<ByteBuffer>> Handle(StateObject state, RequestParameter parameter, CancellationToken cancellationToken)
        {
            await PublishMessage(state, parameter, cancellationToken);
            return new List<ByteBuffer>
            {
                BuildDispose(state, parameter)
            };
        }

        private async Task PublishMessage(StateObject state, RequestParameter parameter, CancellationToken cancellationToken)
        {
            var target = state.Session.Link.Target as Target;
            await state.Session.EventMeshPubSession.Publish(target.Address, Encoding.UTF8.GetString(parameter.Payload), cancellationToken);
        }

        private ByteBuffer BuildDispose(StateObject state, RequestParameter parameter)
        {
            var transfer = parameter.Cmd as Transfer;
            var disposeResult = new Frame { Channel = parameter.Channel, Type = FrameTypes.Amqp };
            var dispose = new Dispose 
            { 
                State = transfer.State, 
                Role = true,
                First = transfer.DeliveryId,
                Last = transfer.DeliveryId,
                Settled = true
            };
            return disposeResult.Serialize(dispose);
        }
    }
}
