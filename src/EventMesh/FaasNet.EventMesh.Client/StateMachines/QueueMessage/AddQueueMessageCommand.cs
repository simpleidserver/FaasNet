using CloudNative.CloudEvents;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.QueueMessage
{
    public class AddQueueMessageCommand : ICommand
    {
        public string Id { get; set; }
        public string Topic { get; set; }
        public CloudEvent Data { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Topic = context.NextString();
            Data = context.NextCloudEvent();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Topic);
            Data.Serialize(context);
        }
    }
}
