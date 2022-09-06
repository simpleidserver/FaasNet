using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines;

namespace FaasNet.EventMesh.Client.StateMachines
{
    public class QueueStateMachine : IStateMachine
    {
        public string Id { get; set; }
        public string QueueName { get; set; }
        public string TopicFilter { get; set; }
        
        public void Apply(ICommand cmd)
        {
            switch(cmd)
            {
                case AddQueueCommand addQueue:
                    QueueName = addQueue.QueueName;
                    TopicFilter = addQueue.TopicFilter;
                    break;
            }
        }

        public void Deserialize(ReadBufferContext context)
        {
            QueueName = context.NextString();
            TopicFilter = context.NextString();
        }

        public byte[] Serialize()
        {
            var writeBufferContext = new WriteBufferContext();
            writeBufferContext.WriteString(QueueName);
            writeBufferContext.WriteString(TopicFilter);
            return writeBufferContext.Buffer.ToArray();
        }
    }

    public class AddQueueCommand : ICommand
    {
        public string QueueName { get; set; }
        public string TopicFilter { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            QueueName = context.NextString();
            TopicFilter = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(QueueName);
            context.WriteString(TopicFilter);
        }
    }
}
