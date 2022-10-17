using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.EventDefinition
{
    public class AddLinkEventDefinitionCommand : ICommand
    {
        public string Id { get; set; }
        public string Vpn { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            Source = context.NextString();
            Target = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
            context.WriteString(Source);
            context.WriteString(Target);
        }
    }
}
