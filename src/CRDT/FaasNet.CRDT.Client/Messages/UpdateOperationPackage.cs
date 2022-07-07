namespace FaasNet.CRDT.Client.Messages
{
    public class UpdateOperationPackage : BaseOperationPackage
    {
        public override OperationTypes Type => OperationTypes.UPDATE;
        public IEntityDelta Delta { get; set; }
    }
}
