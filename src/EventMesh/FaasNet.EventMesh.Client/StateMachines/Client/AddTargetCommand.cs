using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Client
{
    public class AddTargetCommand : ICommand
    {
        public string ClientId { get; set; }
        public string Vpn { get; set; }
        public string Target { get; set; }
        public string EventDefId { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            ClientId = context.NextString();
            Vpn = context.NextString();
            Target = context.NextString();
            EventDefId = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ClientId);
            context.WriteString(Vpn);
            context.WriteString(Target);
            context.WriteString(EventDefId);
        }
    }
}
