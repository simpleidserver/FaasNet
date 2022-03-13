namespace FaasNet.EventMesh.Runtime.Models
{
    public class ClientSessionBridge
    {
        public string Urn { get; set; }
        public int Port { get; set; }
        public string SessionId { get; set; }

        public static ClientSessionBridge Create(string urn, int port, string sessionId)
        {
            return new ClientSessionBridge
            {
                Urn = urn,
                Port = port,
                SessionId = sessionId
            };
        }
    }
}
