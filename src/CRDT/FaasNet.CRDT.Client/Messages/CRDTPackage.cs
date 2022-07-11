namespace FaasNet.CRDT.Client.Messages
{
    public abstract class CRDTPackage
    {
        public string EntityId { get; set; }
        public string Nonce { get; set; }
        public abstract CRDTPackageTypes Type { get; }
    }
}
