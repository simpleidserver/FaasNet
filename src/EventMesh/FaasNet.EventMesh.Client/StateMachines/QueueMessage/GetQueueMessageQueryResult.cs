using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.QueueMessage
{
    public class GetQueueMessageQueryResult : IQueryResult
    {
        public GetQueueMessageQueryResult()
        {
            Success = false;
        }

        public GetQueueMessageQueryResult(QueueMessageQueryResult message)
        {
            Success = true;
            Message = message;
        }

        public bool Success { get; set; }
        public QueueMessageQueryResult Message { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if (Success)
            {
                Message = new QueueMessageQueryResult();
                Message.Deserialize(context);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if (Success) Message.Serialize(context);
        }
    }
}
