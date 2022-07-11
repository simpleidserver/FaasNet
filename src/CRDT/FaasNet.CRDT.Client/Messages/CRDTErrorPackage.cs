namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTErrorPackage : CRDTPackage
    {
        public override CRDTPackageTypes Type => CRDTPackageTypes.ERROR;

        public string Code { get; set; }
    }
}
