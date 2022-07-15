using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTGetPackage : CRDTPackage
    {
        public override CRDTPackageTypes Type => CRDTPackageTypes.GET;

        public string EntityId { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(EntityId);
        }

        public void Extract(ReadBufferContext context)
        {
            EntityId = context.NextString();
        }
    }
}
