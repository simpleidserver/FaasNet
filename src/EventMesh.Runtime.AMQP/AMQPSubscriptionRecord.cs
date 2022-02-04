using EventMesh.Runtime.Models;
using RabbitMQ.Client;

namespace EventMesh.Runtime.AMQP
{
    public class AMQPSubscriptionRecord
    {
        public AMQPSubscriptionRecord(IModel model, ClientSession clientSession, string topicName)
        {
            Model = model;
            ClientSession = clientSession;
            TopicName = topicName;
        }

        public IModel Model { get; set; }
        public ClientSession ClientSession { get; set; }
        public string TopicName { get; set; }
    }
}
