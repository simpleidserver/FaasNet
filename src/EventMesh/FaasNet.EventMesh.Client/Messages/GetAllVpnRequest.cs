using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllVpnRequest : Package
    {
        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
        }
    }
}
