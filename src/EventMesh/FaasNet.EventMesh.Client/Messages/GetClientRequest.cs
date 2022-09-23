using FaasNet.Peer.Client;
using System;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetClientRequest : BaseEventMeshPackage
    {
        public GetClientRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_CLIENT_REQUEST;
        public string ClientId { get; set; }
        public string Vpn { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(ClientId).WriteString(Vpn);
        }

        public GetClientRequest Extract(ReadBufferContext context)
        {
            ClientId = context.NextString();
            Vpn = context.NextString();
            return this;
        }
    }
}
