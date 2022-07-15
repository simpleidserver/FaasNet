using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTGetResultPackage : CRDTPackage
    {
        public override CRDTPackageTypes Type => CRDTPackageTypes.GETRESULT;

        public string Value { get; set; }

        public override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Value);
        }

        public void Extract(ReadBufferContext context)
        {
            Value = context.NextString();
        }
    }
}
