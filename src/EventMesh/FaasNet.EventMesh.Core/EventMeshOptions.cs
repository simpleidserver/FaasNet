namespace FaasNet.EventMesh.Core
{
    public class EventMeshOptions
    {
        public EventMeshOptions()
        {
            Urn = "localhost";
            Port = 4000;
        }

        public string Urn { get; set; }
        public int Port { get; set; }
    }
}
