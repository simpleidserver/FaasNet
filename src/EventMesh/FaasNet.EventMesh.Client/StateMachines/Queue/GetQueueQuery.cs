using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Queue
{
    public class GetQueueQuery : IQuery
    {
        public string QueueName { get; set; }
        public string Vpn { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            QueueName = context.NextString();
            Vpn = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(QueueName);
            context.WriteString(Vpn);
        }
    }
}
