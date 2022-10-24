using FaasNet.Peer.Client;
using System;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAsyncApiRequest : BaseEventMeshPackage
    {
        public GetAsyncApiRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_ASYNC_API_REQUEST;
        public string ClientId { get; set; }
        public string Vpn { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(ClientId);
            context.WriteString(Vpn);
        }

        public GetAsyncApiRequest Extract(ReadBufferContext context)
        {
            ClientId = context.NextString();
            Vpn = context.NextString();
            return this;
        }
    }
}
