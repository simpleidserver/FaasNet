using FaasNet.Peer.Client;
using System.Diagnostics;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetEventDefinitionRequest : BaseEventMeshPackage
    {
        public GetEventDefinitionRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_EVENT_DEFINITION_REQUEST;
        public string Id { get; set; }
        public string Vpn { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
        }

        public GetEventDefinitionRequest Extract(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            return this;
        }
    }
}
