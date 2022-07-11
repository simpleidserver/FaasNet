namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTDeletePackage : CRDTPackage
    {
        public override CRDTPackageTypes Type => CRDTPackageTypes.DELETE;
    }
}
