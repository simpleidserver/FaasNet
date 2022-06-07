namespace FaasNet.EventMesh.Sink
{
    public class SinkOptions
    {
        public SinkOptions()
        {
            EventMeshUrl = "localhost";
            EventMeshPort = 4000;
            Vpn = "default";
            ClientId = "publishClientId";
        }

        public string EventMeshUrl { get; set; }
        public int EventMeshPort { get; set; }
        public string Vpn { get; set; }
        public string ClientId { get; set; }
    }
}
