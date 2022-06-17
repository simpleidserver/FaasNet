namespace FaasNet.EventMesh.Service
{
    public class EventMeshServerOptions
    {
        public int Port { get; set; } = 4000;
        public string ExposedUrl { get; set; } = "localhost";
        public int ExposedPort { get; set; } = 4000;
    }
}
