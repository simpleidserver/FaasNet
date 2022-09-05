using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines;

namespace FaasNet.EventMesh.Client.StateMachines
{
    public class VpnStateMachine : IStateMachine
    {
        public string Id { get; set; }
        public string Description { get; set; }

        public void Apply(ICommand cmd)
        {
            switch (cmd)
            {
                case AddVpnCommand addCmd:
                    Description = addCmd.Description;
                    break;
                case UpdateVpnCommand updateCmd:
                    Description = updateCmd.Description;
                    break;
            }
        }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Description = context.NextString();
        }

        public byte[] Serialize()
        {
            var writeBufferContext = new WriteBufferContext();
            writeBufferContext.WriteString(Id).WriteString(Description);
            return writeBufferContext.Buffer.ToArray();
        }
    }

    public class AddVpnCommand : ICommand
    {
        public string Description { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Description = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Description);
        }
    }

    public class UpdateVpnCommand : ICommand
    {
        public string Description { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Description = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Description);
        }
    }
}
