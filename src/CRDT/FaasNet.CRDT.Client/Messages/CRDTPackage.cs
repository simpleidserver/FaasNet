using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages
{
    public abstract class CRDTPackage : BasePeerPackage
    {
        public static string MAGIC_CODE = "DELTACRDT";
        public override string MagicCode => MAGIC_CODE;
        public override string VersionNumber => "0000";
        public string Nonce { get; set; }
        public abstract CRDTPackageTypes Type { get; }

        public static CRDTPackage Deserialize(ReadBufferContext context, bool ignoreEnvelope = false)
        {
            if(ignoreEnvelope)
            {
                context.NextString();
                context.NextString();
            }

            var nonce = context.NextString();
            var type = CRDTPackageTypes.Deserialize(context);
            if(type == CRDTPackageTypes.DELTA)
            {
                var result = new CRDTDeltaPackage { Nonce = nonce };
                result.Extract(context);
                return result;
            }

            if (type == CRDTPackageTypes.DELETE) return new CRDTDeletePackage { Nonce = nonce };
            if (type == CRDTPackageTypes.RESULT) return new CRDTResultPackage { Nonce = nonce };
            if (type == CRDTPackageTypes.ERROR)
            {
                var result = new CRDTErrorPackage { Nonce = nonce };
                result.Extract(context);
                return result;
            }

            if(type == CRDTPackageTypes.SYNC)
            {
                var result = new CRDTSyncPackage { Nonce = nonce };
                result.Extract(context);
                return result;
            }

            if (type == CRDTPackageTypes.SYNCRESULT)
            {
                var result = new CRDTSyncResultPackage { Nonce = nonce };
                result.Extract(context);
                return result;
            }

            if (type == CRDTPackageTypes.GET)
            {
                var result = new CRDTGetPackage { Nonce = nonce };
                result.Extract(context);
                return result;
            }

            if (type == CRDTPackageTypes.GETRESULT)
            {
                var result = new CRDTGetResultPackage { Nonce = nonce };
                result.Extract(context);
                return result;
            }

            return null;
        }

        public override void SerializeBody(WriteBufferContext context)
        {
            context.WriteString(Nonce);
            Type.Serialize(context);
            SerializeAction(context);
        }

        public abstract void SerializeAction(WriteBufferContext context);
    }
}
