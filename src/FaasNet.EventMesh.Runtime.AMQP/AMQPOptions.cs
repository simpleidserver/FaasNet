using RabbitMQ.Client;
using System;

namespace FaasNet.EventMesh.Runtime.AMQP
{
    public class AMQPOptions : BaseBrokerOptions
    {
        public AMQPOptions()
        {
            ConnectionFactory = (o) =>
            {
                o.HostName = "127.0.0.1";
                o.Port = 5672;
                o.UserName = "guest";
                o.Password = "guest";
            };
            TopicName = "amq.topic";
            QueueName = "streamQueue";
            BrokerName = "amqp";
            Source = "urn:eventmesh";
        }

        public Action<ConnectionFactory> ConnectionFactory { get; set; }
        public string TopicName { get; set; }
        public string QueueName { get; set; }
        public string Source { get; set; }
    }
}
