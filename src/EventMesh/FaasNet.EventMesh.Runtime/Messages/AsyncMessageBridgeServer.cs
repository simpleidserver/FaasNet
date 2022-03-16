namespace FaasNet.EventMesh.Runtime.Messages
{
    public class AsyncMessageBridgeServer
    {
        #region Properties

        public string Vpn { get; set; }
        public string Urn { get; set; }
        public int Port { get; set; }

        #endregion

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(Urn);
            context.WriteInteger(Port);
        }

        public static AsyncMessageBridgeServer Deserialize(ReadBufferContext context)
        {
            return new AsyncMessageBridgeServer
            {
                Vpn = context.NextString(),
                Urn = context.NextString(),
                Port = context.NextInt()
            };
        }
    }
}
