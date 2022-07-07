namespace FaasNet.CRDT.Client.Messages
{
    public abstract class BaseOperationPackage
    {
        public abstract OperationTypes Type { get; }
    }
}
