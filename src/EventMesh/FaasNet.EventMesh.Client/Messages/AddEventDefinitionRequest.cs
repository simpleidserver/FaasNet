using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddEventDefinitionRequest : BaseEventMeshPackage
    {
        public AddEventDefinitionRequest(string seq) : base(seq)
        {
        }

        public string Id { get; set; }
        public string Vpn { get; set; }
        public string JsonSchema { get; set; }

        public override EventMeshCommands Command => EventMeshCommands.ADD_EVENT_DEFINITION_REQUEST;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
            context.WriteString(JsonSchema);
        }

        public AddEventDefinitionRequest Extract(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            JsonSchema = context.NextString();
            return this;
        }
    }
}
