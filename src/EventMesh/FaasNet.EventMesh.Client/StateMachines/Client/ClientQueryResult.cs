using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.StateMachines.Client
{
    public class ClientQueryResult : IQueryResult
    {
        public string Id { get; set; }
        public string ClientSecret { get; set; }
        public string Vpn { get; set; }
        public int SessionExpirationTimeMS { get; set; }
        public ICollection<ClientPurposeTypes> Purposes { get; set; } = new List<ClientPurposeTypes>();

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            ClientSecret = context.NextString();
            Vpn = context.NextString();
            SessionExpirationTimeMS = context.NextInt();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++) Purposes.Add((ClientPurposeTypes)context.NextInt());
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(ClientSecret);
            context.WriteString(Vpn);
            context.WriteInteger(SessionExpirationTimeMS);
            context.WriteInteger(Purposes.Count);
            foreach (var purpose in Purposes) context.WriteInteger((int)purpose);
        }
    }
}
