namespace FaasNet.CRDT.Client.Messages
{
    public class CRDTDeltaPackage : CRDTPackage
    {
        public override CRDTPackageTypes Type => CRDTPackageTypes.DELTA;

        public IEntityDelta Delta { get; set; }
    }
}
