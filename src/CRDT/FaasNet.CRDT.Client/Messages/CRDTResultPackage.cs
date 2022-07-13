using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTResultPackage : CRDTPackage
    {
        public override CRDTPackageTypes Type => CRDTPackageTypes.RESULT;

        public override void SerializeAction(WriteBufferContext context)
        {

        }
    }
}
