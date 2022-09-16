using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.StateMachines.Vpn
{
    public class GetAllVpnQuery : IQuery
    {
        public void Deserialize(ReadBufferContext context) { }

        public void Serialize(WriteBufferContext context) { }
    }

    public class GetAllVpnQueryResult : IQueryResult
    {
        public ICollection<VpnQueryResult> Vpns { get; set; } = new List<VpnQueryResult>();

        public void Deserialize(ReadBufferContext context)
        {
            var nb = context.NextInt();
            for(var i = 0; i < nb; i++)
            {
                var newVpn = new VpnQueryResult();
                newVpn.Deserialize(context);
                Vpns.Add(newVpn);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteInteger(Vpns.Count);
            foreach(var vpn in Vpns)
                vpn.Serialize(context);
        }
    }
}
