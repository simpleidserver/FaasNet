using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Queue
{
    public class SearchQueuesQuery : IQuery
    {
        public string TopicMessage { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            TopicMessage = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(TopicMessage);
        }
    }
}
