namespace FaasNet.EventMesh.Runtime.Kafka
{
    public class KafkaSubscriptionRecord
    {
        public KafkaSubscriptionRecord(string topicName, string brokerName, string clientId, string clientSessionId, KafkaListener listener)
        {
            TopicName = topicName;
            BrokerName = brokerName;
            ClientId = clientId;
            ClientSessionId = clientSessionId;
            Listener = listener;
        }

        public string TopicName { get; set; }
        public string BrokerName { get; set; }
        public string ClientId { get; set; }
        public string ClientSessionId { get; set; }
        public KafkaListener Listener { get; set; }
    }
}
