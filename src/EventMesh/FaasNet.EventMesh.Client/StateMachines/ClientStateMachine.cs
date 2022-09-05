using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.StateMachines
{
    public class ClientStateMachine : IStateMachine
    {
        public ClientStateMachine()
        {
            Purposes = new List<ClientPurposeTypes>();
        }

        public string Id { get; set; }
        public string Vpn { get; set; }
        public ICollection<ClientPurposeTypes> Purposes { get; set; }

        public void Apply(ICommand cmd)
        {
            switch(cmd)
            {
                case AddClientCommand addClient:
                    Vpn = addClient.Vpn;
                    Purposes = addClient.Purposes;
                    break;
            }
        }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            var nb = context.NextInt();
            for(var i = 0; i < nb; i++)
                Purposes.Add((ClientPurposeTypes)context.NextInt());
        }

        public byte[] Serialize()
        {
            var writeBufferContext = new WriteBufferContext();
            writeBufferContext.WriteString(Id);
            writeBufferContext.WriteString(Vpn);
            writeBufferContext.WriteInteger(Purposes.Count);
            foreach (var purpose in Purposes) writeBufferContext.WriteInteger((int)purpose);
            return writeBufferContext.Buffer.ToArray();
        }
    }
    public enum ClientPurposeTypes
    {
        PUBLISH = 0,
        SUBSCRIBE = 1
    }

    public class AddClientCommand : ICommand
    {
        public AddClientCommand()
        {
            Purposes = new List<ClientPurposeTypes>();
        }

        public string Vpn { get; set; }
        public ICollection<ClientPurposeTypes> Purposes { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Vpn = context.NextString();
            var result = new List<ClientPurposeTypes>();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++) result.Add((ClientPurposeTypes)context.NextInt());
            Purposes = result;
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteInteger(Purposes.Count);
            foreach (var purpose in Purposes) context.WriteInteger((int)purpose);
        }
    }
}
