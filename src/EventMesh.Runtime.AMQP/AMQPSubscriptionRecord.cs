using EventMesh.Runtime.Models;
using RabbitMQ.Client;

namespace EventMesh.Runtime.AMQP
{
    public class AMQPSubscriptionRecord
    {
        public AMQPSubscriptionRecord(IModel model, string clientId, ClientSession clientSession, string topicName)
        {
            Model = model;
            ClientId = clientId;
            ClientSession = clientSession;
            TopicName = topicName;
        }

        public IModel Model { get; set; }
        public string ClientId { get; set; }
        public ClientSession ClientSession { get; set; }
        public string TopicName { get; set; }
    }
}
