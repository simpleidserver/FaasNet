using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTDeletePackage : CRDTPackage
    {
        public override CRDTPackageTypes Type => CRDTPackageTypes.DELETE;

        protected override void SerializeAction(WriteBufferContext context) { }
    }
}
