namespace EventMesh.Runtime.Messages
{
    public class AddBridgeRequest : Package
    {
        #region Properties

        public string Urn { get; set; }
        public int Port { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Urn);
            context.WriteInteger(Port);
        }

        public void Extract(ReadBufferContext context)
        {
            Urn = context.NextString();
            Port = context.NextInt();
        }
    }
}
