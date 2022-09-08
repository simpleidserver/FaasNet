namespace FaasNet.EventMesh.UI
{
    public class EventMeshUIOptions
    {
        public string EventMeshUrl { get; set; } = "localhost";
        public int EventMeshPort { get; set; } = 5000;
        public int RequestTimeoutMS { get; set; } = 1000;
    }
}
