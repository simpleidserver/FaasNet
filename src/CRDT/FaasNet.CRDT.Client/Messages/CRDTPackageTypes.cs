using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTPackageTypes : BaseEnumeration
    {
        public static CRDTPackageTypes DELETE = new CRDTPackageTypes(0, "DELETE");
        public static CRDTPackageTypes DELTA = new CRDTPackageTypes(1, "APPLY_DETLA");
        public static CRDTPackageTypes ERROR = new CRDTPackageTypes(2, "ERROR");

        public CRDTPackageTypes(int code)
        {
            Init<CRDTPackageTypes>(code);
        }

        public CRDTPackageTypes(int code, string description) : base(code, description)
        {
        }

        public static CRDTPackageTypes Deserialize(ReadBufferContext context)
        {
            return new CRDTPackageTypes(context.NextInt());
        }
    }
}