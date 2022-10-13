using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.EventDefinition
{
    public class UpdateEventDefinitionCommand : ICommand
    {
        public string Vpn { get; set; }
        public string Id { get; set; }
        public string JsonSchema { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Vpn = context.NextString();
            Id = context.NextString();
            JsonSchema = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(Id);
            context.WriteString(JsonSchema);
        }
    }
}
