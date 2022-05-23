namespace FaasNet.EventMesh.Protocols.AMQP
{
    public class EventMeshAMQPOptions
    {
        public EventMeshAMQPOptions()
        {
            Port = 5672;
            EventMeshVpn = Client.Constants.DefaultVpn;
            EventMeshUrl = Client.Constants.DefaultUrl;
            EventMeshPort = Client.Constants.DefaultPort;
        }

        public int Port { get; set; }
        public string EventMeshVpn { get; set; }
        public string EventMeshUrl { get; set; }
        public int EventMeshPort { get; set; }
    }
}
