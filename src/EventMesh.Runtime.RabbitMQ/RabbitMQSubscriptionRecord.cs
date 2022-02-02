using RabbitMQ.Client;

namespace EventMesh.Runtime.RabbitMQ
{
    public class RabbitMQSubscriptionRecord
    {
        public RabbitMQSubscriptionRecord(string topic, string routingKey, IModel model)
        {
            Topic = topic;
            RoutingKey = routingKey;
            Model = model;
        }

        public string Topic { get; set; }
        public string RoutingKey { get; set; }
        public IModel Model { get; set; }
    }
}
