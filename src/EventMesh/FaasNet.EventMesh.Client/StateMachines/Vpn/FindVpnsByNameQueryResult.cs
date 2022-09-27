using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.StateMachines.Vpn
{
    public class FindVpnsByNameQueryResult : IQueryResult
    {
        public IEnumerable<string> Content { get; set; } = new List<string>();

        public void Deserialize(ReadBufferContext context)
        {
            var nb = context.NextInt();
            var content = new List<string>();
            for(var i = 0; i < nb; i++)
                content.Add(context.NextString());
            Content = content;
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteInteger(Content.Count());
            foreach (var record in Content) context.WriteString(record);
        }
    }
}
