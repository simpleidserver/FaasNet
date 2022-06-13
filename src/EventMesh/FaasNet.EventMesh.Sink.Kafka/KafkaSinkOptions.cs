using FaasNet.EventMesh.Plugin;
using System;

namespace FaasNet.EventMesh.Sink.Kafka
{
    public class KafkaSinkOptions : SinkOptions
    {
        public KafkaSinkOptions()
        {
            JobId = "EventMeshKafka";
            GroupId = "EventMeshKafka";
            GetMetadataTimeout = TimeSpan.FromSeconds(2);
            ListenKafkaTopicTimerMS = 5 * 1000;
        }

        /// <summary>
        /// List of brokers.
        /// </summary>
        [PluginEntryOptionProperty("kafkaBootstrapServers", "List of brokers.")]
        public string BootstrapServers { get; set; }
        /// <summary>
        /// Client group identifier.
        /// </summary>
        [PluginEntryOptionProperty("kafkaGroupId", "Client group identifier.")]
        public string GroupId { get; set; }
        /// <summary>
        /// Job identifier.
        /// </summary>
        [PluginEntryOptionProperty("jobId", "Job identifier.")]
        public string JobId { get; set; }
        /// <summary>
        /// Timeout in MS to fetch metadata from Kafka.
        /// </summary>
        [PluginEntryOptionProperty("kafkaMetadataTimeout", "Job identifier.")]
        public TimeSpan GetMetadataTimeout { get; set; }
        /// <summary>
        /// Timer in MS used to fetch kafka topics.
        /// </summary>
        [PluginEntryOptionProperty("kafkaTopicTimerMS", "Timer in MS used to fetch kafka topics..")]
        public int ListenKafkaTopicTimerMS { get; set; }
    }
}
