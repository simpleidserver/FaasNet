using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTPackageTypes : BaseEnumeration
    {
        public static CRDTPackageTypes DELETE = new CRDTPackageTypes(0, "DELETE");
        public static CRDTPackageTypes DELTA = new CRDTPackageTypes(1, "APPLY_DETLA");
        public static CRDTPackageTypes SYNC = new CRDTPackageTypes(2, "SYNC");
        public static CRDTPackageTypes SYNCRESULT = new CRDTPackageTypes(3, "SYNCRESULTT");
        public static CRDTPackageTypes ERROR = new CRDTPackageTypes(4, "ERROR");
        public static CRDTPackageTypes RESULT = new CRDTPackageTypes(5, "RESULT");
        public static CRDTPackageTypes GET = new CRDTPackageTypes(6, "GET");
        public static CRDTPackageTypes GETRESULT = new CRDTPackageTypes(7, "GETRESULT");

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