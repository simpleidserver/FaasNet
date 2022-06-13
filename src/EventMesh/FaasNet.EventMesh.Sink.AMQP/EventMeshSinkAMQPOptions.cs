using FaasNet.EventMesh.Plugin;

namespace FaasNet.EventMesh.Sink.AMQP
{
    public class EventMeshSinkAMQPOptions : SinkOptions
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

        /// <summary>
        /// Name of the topic exchange.
        /// </summary>
        [PluginEntryOptionProperty("amqpTopicName", "Name of the topic exchange.")]
        public string TopicName { get; set; }
        /// <summary>
        /// Job identifier.
        /// </summary>
        [PluginEntryOptionProperty("jobId", "Job identifier.")]
        public string JobId { get; set; }
        /// <summary>
        /// AMQP server host.
        /// </summary>
        [PluginEntryOptionProperty("amqpHost", "AMQP server host.")]
        public string AMQPHostName { get; set; }
        /// <summary>
        /// AMQP server port.
        /// </summary>
        [PluginEntryOptionProperty("amqpPort", "AMQP server port.")]
        public int AMQPPort { get; set; }
        /// <summary>
        /// AMQP username.
        /// </summary>
        [PluginEntryOptionProperty("amqpUserName", "AMQP username.")]
        public string AMQPUserName { get; set; }
        /// <summary>
        /// AMQP password.
        /// </summary>
        [PluginEntryOptionProperty("amqpPassword", "AMQP password.")]
        public string AMQPPassword { get; set; }
    }
}
