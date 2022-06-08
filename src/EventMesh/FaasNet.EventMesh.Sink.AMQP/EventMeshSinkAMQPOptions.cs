using RabbitMQ.Client;
using System;

namespace FaasNet.EventMesh.Sink.AMQP
{
    public class EventMeshSinkAMQPOptions
    {
        public EventMeshSinkAMQPOptions()
        {
            AMQPHostName = "127.0.0.1";
            AMQPPort = 5672;
            AMQPUserName = "guest";
            AMQPPassword = "guest";
            TopicName = "amq.topic";
            JobId = "AMQPTopic";
        }

        public string TopicName { get; set; }
        public string JobId { get; set; }
        public string AMQPHostName { get; set; }
        public int AMQPPort { get; set; }
        public string AMQPUserName { get; set; }
        public string AMQPPassword { get; set; }
    }
}
