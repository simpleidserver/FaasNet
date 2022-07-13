using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTDeletePackage : CRDTPackage
    {
        public override CRDTPackageTypes Type => CRDTPackageTypes.DELETE;

        public string EntityId { get; set; }

        public override void SerializeAction(WriteBufferContext context) 
        { 
            context.WriteString(EntityId);
        }
    }
}
