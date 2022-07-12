using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTErrorPackage : CRDTPackage
    {
        public override CRDTPackageTypes Type => CRDTPackageTypes.ERROR;

        public string Code { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Code);
        }

        public void Extract(ReadBufferContext context)
        {
            Code = context.NextString();
        }
    }
}
