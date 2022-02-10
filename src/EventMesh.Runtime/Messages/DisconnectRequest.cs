namespace EventMesh.Runtime.Messages
{
    public class DisconnectRequest : Package
    {
        #region Properties

        public string ClientId { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(ClientId);
        }

        public void Extract(ReadBufferContext context)
        {
            ClientId = context.NextString();
        }
    }
}
