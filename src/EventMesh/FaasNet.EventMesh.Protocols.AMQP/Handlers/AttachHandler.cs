﻿using Amqp;
using Amqp.Framing;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Protocols.AMQP.Extensions;
using FaasNet.EventMesh.Protocols.AMQP.Framing;
using Microsoft.Extensions.Options;
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

        public async Task<RequestResult> Handle(StateObject state, RequestParameter parameter, CancellationToken cancellationToken)
        {
            var attachCmd = parameter.Cmd as Attach;
            var target = attachCmd.Target as Target;
            var source = attachCmd.Source as Source;
            state.Session.Link = attachCmd;
            if (target != null && !string.IsNullOrWhiteSpace(target.Address)) state.Session.EventMeshPubSession = await CreatePubSession(state.Session, cancellationToken);
            if (source != null && !string.IsNullOrWhiteSpace(source.Address)) state.Session.EventMeshSubSession = await CreateSubSession(parameter, state, cancellationToken);
            return RequestResult.Ok(new ByteBuffer[]
            {
                BuildAttachResponse(parameter.Channel, attachCmd),
                BuildFrameResponse(attachCmd, parameter.Channel, attachCmd)
            });
        }

        private async Task<SubscriptionResult> CreateSubSession(RequestParameter parameter, StateObject state, CancellationToken cancellationToken)
        {
            var session = state.Session;
            var attachCmd = parameter.Cmd as Attach;
            var source = attachCmd.Source as Source;
            var evtMeshClient = new EventMeshClient(_options.EventMeshUrl, _options.EventMeshPort);
            var subSession = await evtMeshClient.CreateSubSession(_options.EventMeshVpn, session.ClientId, null, cancellationToken);
            var subscriptionResult = subSession.DirectSubscribe(source.Address, (cb) =>
            {
                var message = new Message(cb.Data.ToString());
                var messageByteBuffer = message.Serialize();
                var result = new Frame { Channel = parameter.Channel, Type = FrameTypes.Amqp };
                var transfer = new Transfer 
                { 
                    Handle = attachCmd.Handle,
                    DeliveryId = state.Session.DeliveryId                    
                };
                var byteBuffer = result.Serialize(transfer, messageByteBuffer);
                state.WorkSocket.Send(byteBuffer.Buffer, byteBuffer.Offset, byteBuffer.Length, 0);
                state.Session.DeliveryId = state.Session.DeliveryId + 1;
            }, cancellationToken);
            return subscriptionResult;
        }

        private async Task<IEventMeshClientPubSession> CreatePubSession(StateSessionObject session, CancellationToken cancellationToken)
        {
            var evtMeshClient = new EventMeshClient(_options.EventMeshUrl, _options.EventMeshPort);
            return await evtMeshClient.CreatePubSession(_options.EventMeshVpn, session.ClientId, null, cancellationToken);
        }

        private static ByteBuffer BuildAttachResponse(ushort channel, Attach requestAttach)
        {
            var result = new Frame { Channel = channel, Type = FrameTypes.Amqp };
            var attach = new Attach 
            { 
                LinkName = requestAttach.LinkName, 
                Handle = requestAttach.Handle,
                Role = !requestAttach.Role,
                Source = requestAttach.Source,
                Target = requestAttach.Target
            };
            return result.Serialize(attach);
        }

        private ByteBuffer BuildFrameResponse(Attach attachCmd, ushort channel, Attach requestAttach)
        {
            var target = attachCmd.Target as Target;
            var isReceiverEdp = target != null && !string.IsNullOrWhiteSpace(target.Address);
            var result = new Frame { Channel = channel, Type = FrameTypes.Amqp };
            var flow = new Flow { Handle = requestAttach.Handle };
            if (isReceiverEdp)
            {
                flow.LinkCredit = _options.SessionLinkCredit;
            }

            return result.Serialize(flow);
        }
    }
}
