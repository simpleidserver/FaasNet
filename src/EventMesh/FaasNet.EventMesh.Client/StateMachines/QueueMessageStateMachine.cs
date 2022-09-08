using CloudNative.CloudEvents;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines;

namespace FaasNet.EventMesh.Client.StateMachines
{
    public class QueueMessageStateMachine : IStateMachine
    {
        public string Id { get; set; }
        public string Topic { get; set; }
        public bool HasData { get; set; }
        public CloudEvent Data { get; set; }

        public void Apply(ICommand cmd)
        {
            switch(cmd)
            {
                case AddQueueMessageCommand addQueue:
                    Topic = addQueue.Topic;
                    Data = addQueue.Data;
                    break;
            }
        }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Topic = context.NextString();
            var hasData = context.NextBoolean();
            if (hasData) Data = context.NextCloudEvent();
        }

        public byte[] Serialize()
        {
            var context = new WriteBufferContext();
            context.WriteString(Id);
            context.WriteString(Topic);
            context.WriteBoolean(Data != null);
            if (Data != null) Data.Serialize(context);
            return context.Buffer.ToArray();
        }
    }

    public class AddQueueMessageCommand : ICommand
    {
        public string Topic { get; set; }
        public CloudEvent Data { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Topic = context.NextString();
            Data = context.NextCloudEvent();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Topic);
            Data.Serialize(context);
        }
    }
}
