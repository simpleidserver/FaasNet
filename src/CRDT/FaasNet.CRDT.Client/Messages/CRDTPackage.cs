using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages
{
    public abstract class CRDTPackage : BasePeerPackage
    {
        public static string MAGIC_CODE = "DELTACRDT";
        public override string MagicCode => MAGIC_CODE;
        public override string VersionNumber => "0000";
        public string PeerId { get; set; }
        public string EntityId { get; set; }
        public string Nonce { get; set; }
        public abstract CRDTPackageTypes Type { get; }

        public static CRDTPackage Deserialize(ReadBufferContext context)
        {
            var peerId = context.NextString();
            var entityId = context.NextString();
            var nonce = context.NextString();
            var type = CRDTPackageTypes.Deserialize(context);
            if(type == CRDTPackageTypes.DELTA)
            {
                var result = new CRDTDeltaPackage { PeerId = peerId, EntityId = entityId, Nonce = nonce };
                result.Extract(context);
                return result;
            }

            if (type == CRDTPackageTypes.DELETE) return new CRDTDeletePackage { PeerId = peerId, EntityId = entityId, Nonce = nonce };
            if (type == CRDTPackageTypes.ERROR)
            {
                var result = new CRDTErrorPackage { PeerId = peerId, EntityId = entityId, Nonce = nonce };
                result.Extract(context);
                return result;
            }

            return null;
        }

        protected override void SerializeBody(WriteBufferContext context)
        {
            context.WriteString(PeerId);
            context.WriteString(EntityId);
            context.WriteString(Nonce);
            Type.Serialize(context);
            SerializeAction(context);
        }

        protected abstract void SerializeAction(WriteBufferContext context);
    }
}
