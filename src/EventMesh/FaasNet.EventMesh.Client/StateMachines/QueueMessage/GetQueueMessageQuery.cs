using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.QueueMessage
{
    public class GetQueueMessageQuery : IQuery
    {
        public string QueueName { get; set; }
        public int Offset { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            QueueName = context.NextString();
            Offset = context.NextInt();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(QueueName);
            context.WriteInteger(Offset);
        }
    }
}
