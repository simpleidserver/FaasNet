namespace FaasNet.EventMesh.Runtime.Kafka
{
    public class KafkaOptions : BaseBrokerOptions
    {
        public KafkaOptions()
        {
            BrokerName = "kafka";
            BootstrapServers = "localhost:29092";
        }

        public string BootstrapServers { get; set; }
    }
}
