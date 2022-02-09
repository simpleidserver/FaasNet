namespace EventMesh.Runtime.Models
{
    public class ClientSessionBridge
    {
        public string Urn { get; set; }

        public static ClientSessionBridge Create(string urn)
        {
            return new ClientSessionBridge
            {
                Urn = urn
            };
        }
    }
}
