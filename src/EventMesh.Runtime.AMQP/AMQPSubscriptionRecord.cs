using RabbitMQ.Client;

namespace EventMesh.Runtime.AMQP
{
    public class AMQPSubscriptionRecord
    {
        public AMQPSubscriptionRecord(string topicName, string brokerName, string clientId, string clientSessionId, IModel channel, string consumerTag)
        {
            TopicName = topicName;
            BrokerName = brokerName;
            ClientId = clientId;
            ClientSessionId = clientSessionId;
            Channel = channel;
            ConsumerTag = consumerTag;
        }

        public string TopicName { get; set; }
        public string BrokerName { get; set; }
        public string ClientId { get; set; }
        public string ClientSessionId { get; set; }
        public IModel Channel { get; set; }
        public string ConsumerTag { get; set; }
    }
}
