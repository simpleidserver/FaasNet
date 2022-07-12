namespace FaasNet.Peer.Client
{
    public abstract class BasePeerPackage
    {
        public abstract string MagicCode { get; }
        public abstract string VersionNumber { get; }

        public void SerializeEnvelope(WriteBufferContext context)
        {
            context.WriteString(MagicCode);
            context.WriteString(VersionNumber);
            SerializeBody(context);
        }

        protected abstract void SerializeBody(WriteBufferContext context);
    }
}
