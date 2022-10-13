using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class UpdateEventDefinitionRequest : BaseEventMeshPackage
    {
        public UpdateEventDefinitionRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.UPDATE_EVENT_DEFINITION_REQUEST;
        public string Vpn { get; set; }
        public string Id { get; set; }
        public string JsonSchema { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(Id);
            context.WriteString(JsonSchema);
        }

        public UpdateEventDefinitionRequest Extract(ReadBufferContext context)
        {
            Vpn = context.NextString();
            Id = context.NextString();
            JsonSchema = context.NextString();
            return this;
        }
    }
}
