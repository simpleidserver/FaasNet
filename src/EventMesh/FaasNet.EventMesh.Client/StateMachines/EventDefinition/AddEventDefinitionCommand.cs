using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.EventDefinition
{
    public class AddEventDefinitionCommand : ICommand
    {
        public string Id { get; set; }
        public string Vpn { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string JsonSchema { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            Topic = context.NextString();
            Description = context.NextString();
            JsonSchema = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
            context.WriteString(Topic);
            context.WriteString(Description);
            context.WriteString(JsonSchema);
        }
    }
}
