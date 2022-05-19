namespace FaasNet.EventMesh.Seed
{
    public class SeedOptions
    {
        public SeedOptions()
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
