namespace FaasNet.EventMesh.Runtime.Messages
{
    public class AsyncMessageBridgeServer
    {
        #region Properties

        public string Urn { get; set; }
        public int Port { get; set; }

        #endregion

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Urn);
            context.WriteInteger(Port);
        }

        public static AsyncMessageBridgeServer Deserialize(ReadBufferContext context)
        {
            return new AsyncMessageBridgeServer
            {
                Urn = context.NextString(),
                Port = context.NextInt()
            };
        }
    }
}
