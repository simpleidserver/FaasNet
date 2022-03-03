namespace FaasNet.EventMesh.Runtime.Models
{
    public class BridgeServer
    {
        public string Urn { get; set; }
        public int Port { get; set; }

        public static BridgeServer Create(string urn, int port)
        {
            return new BridgeServer
            {
                Urn = urn,
                Port = port
            };
        }
    }
}
