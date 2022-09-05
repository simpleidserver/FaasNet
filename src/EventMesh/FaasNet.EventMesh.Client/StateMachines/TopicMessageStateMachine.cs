using CloudNative.CloudEvents;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines;

namespace FaasNet.EventMesh.Client.StateMachines
{
    public class TopicMessageStateMachine : IStateMachine
    {
        public string Id { get; set; }
        public CloudEvent Message { get; set; }
        
        public void Apply(ICommand cmd)
        {
            switch(cmd)
            {
                case AddVpnMessageCommand addMessage:
                    Message = addMessage.Message;
                    break;
            }
        }

        public void Deserialize(ReadBufferContext context)
        {
            Message = context.NextCloudEvent();
        }

        public byte[] Serialize()
        {
            var writeBufferContext = new WriteBufferContext();
            Message.Serialize(writeBufferContext);
            return writeBufferContext.Buffer.ToArray();
        }
    }

    public class AddVpnMessageCommand : ICommand
    {
        public CloudEvent Message { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Message = context.NextCloudEvent();
        }

        public void Serialize(WriteBufferContext context)
        {
            Message.Serialize(context);
        }
    }
}
