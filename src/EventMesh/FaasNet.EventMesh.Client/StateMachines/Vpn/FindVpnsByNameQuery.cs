using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Vpn
{
    public class FindVpnsByNameQuery : IQuery
    {
        public string Name { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Name = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Name);
        }
    }
}
