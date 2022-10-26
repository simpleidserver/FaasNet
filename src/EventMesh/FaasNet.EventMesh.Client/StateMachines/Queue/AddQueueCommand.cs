using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Queue
{
    public class AddQueueCommand : ICommand
    {
        public string Vpn { get; set; }
        public string QueueName { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Vpn = context.NextString();
            QueueName = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(QueueName);
        }
    }
}
