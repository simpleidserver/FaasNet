using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Client
{
    public class AddSourceCommand : ICommand
    {
        public string ClientId { get; set; }
        public string Vpn { get; set; }
        public string Source { get; set; }
        public string EventDefId { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            ClientId = context.NextString();
            Vpn = context.NextString();
            Source = context.NextString();
            EventDefId = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ClientId);
            context.WriteString(Vpn);
            context.WriteString(Source);
            context.WriteString(EventDefId);
        }
    }
}
