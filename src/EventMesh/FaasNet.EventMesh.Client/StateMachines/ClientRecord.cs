using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client.Commands;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.StateMachines
{
    public class ClientRecord : IEntityRecord
    {
        public ClientRecord()
        {
            Purposes = new List<ClientPurposeTypes>();
        }

        public string Id { get; set; }
        public string Vpn { get; set; }
        public ICollection<ClientPurposeTypes> Purposes { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            var nbPurposes = context.NextInt();
            for (var i = 0; i < nbPurposes; i++) Purposes.Add((ClientPurposeTypes)context.NextInt());
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
            context.WriteInteger(Purposes.Count);
            foreach (var purpose in Purposes) context.WriteInteger((int)purpose);
        }
    }
}
