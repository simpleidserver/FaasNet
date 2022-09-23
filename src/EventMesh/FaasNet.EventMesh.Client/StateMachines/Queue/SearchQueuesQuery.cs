using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Queue
{
    public class SearchQueuesQuery : IQuery
    {
        public string Vpn { get; set; }
        public string TopicMessage { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Vpn = context.NextString();
            TopicMessage = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(TopicMessage);
        }
    }
}
