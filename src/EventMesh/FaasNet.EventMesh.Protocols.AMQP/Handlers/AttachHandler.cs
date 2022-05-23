using Amqp;
using Amqp.Framing;
using Amqp.Types;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.AMQP.Handlers
{
    public class AttachHandler : IRequestHandler
    {
        private readonly EventMeshAMQPOptions _options;

        public AttachHandler(IOptions<EventMeshAMQPOptions> options)
        {
            _options = options.Value;
        }

        public string RequestName => "amqp:attach:list";

        public async Task<IEnumerable<ByteBuffer>> Handle(StateObject state, DescribedList cmd, byte[] payload, ushort channel, CancellationToken cancellationToken)
        {
            var attachCmd = cmd as Attach;
            var target = attachCmd.Target as Target;
            if(target != null && !string.IsNullOrWhiteSpace(target.Address)) state.Session.EventMeshPubSession = await CreatePubSession(state.Session, cancellationToken);
            return new ByteBuffer[]
            {
                BuildAttachResponse(channel, attachCmd),
                BuildFrameResponse(channel, attachCmd)
            };
        }

        private async Task CreateSubSession(StateSessionObject session, CancellationToken cancellationToken)
        {
            var evtMeshClient = new EventMeshClient(_options.EventMeshUrl, _options.EventMeshPort);
            var subSession = await evtMeshClient.CreateSubSession(_options.EventMeshVpn, session.ClientId, cancellationToken);
        }

        private async Task<IEventMeshClientPubSession> CreatePubSession(StateSessionObject session, CancellationToken cancellationToken)
        {
            var evtMeshClient = new EventMeshClient(_options.EventMeshUrl, _options.EventMeshPort);
            return await evtMeshClient.CreatePubSession(_options.EventMeshVpn, session.ClientId, cancellationToken);
        }

        private static ByteBuffer BuildAttachResponse(ushort channel, Attach requestAttach)
        {
            var result = new Frame { Channel = channel, Type = FrameTypes.Amqp };
            var attach = new Attach { LinkName = requestAttach.LinkName };
            return result.Serialize(attach);
        }

        private static ByteBuffer BuildFrameResponse(ushort channel, Attach requestAttach)
        {
            var result = new Frame { Channel = channel, Type = FrameTypes.Amqp };
            var flow = new Flow { LinkCredit = 1, Handle = requestAttach.Handle };
            return result.Serialize(flow);
        }
    }
}
