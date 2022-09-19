using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Vpn
{
    public class GetVpnQueryResult : IQueryResult
    {
        public GetVpnQueryResult()
        {
            Success = false;
        }

        public GetVpnQueryResult(VpnQueryResult vpn)
        {
            Success = true;
            Vpn = vpn;
        }

        public bool Success { get; set; }
        public VpnQueryResult Vpn { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if (Success)
            {
                Vpn = new VpnQueryResult();
                Vpn.Deserialize(context);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if (Success) Vpn.Serialize(context);
        }
    }
}
