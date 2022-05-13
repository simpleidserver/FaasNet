using FaasNet.RaftConsensus.Client.Messages;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddClientRequest : Package
    {
        public AddClientRequest()
        {
            Purposes = new List<UserAgentPurpose>();
        }

        public string Vpn { get; set; }
        public string ClientId { get; set; }
        public ICollection<UserAgentPurpose> Purposes { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Vpn);
            context.WriteString(ClientId);
            context.WriteInteger(Purposes.Count());
            foreach (var purpose in Purposes) purpose.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            Vpn = context.NextString();
            ClientId = context.NextString();
            var nbPurposes = context.NextInt();
            for (int i = 0; i < nbPurposes; i++) Purposes.Add(UserAgentPurpose.Deserialize(context)); 
        }
    }
}
