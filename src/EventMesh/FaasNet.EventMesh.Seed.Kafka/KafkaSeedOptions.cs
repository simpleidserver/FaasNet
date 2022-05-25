using System;

namespace FaasNet.EventMesh.Seed.Kafka
{
    public class KafkaSeedOptions
    {
        public KafkaSeedOptions()
        {
            JobId = "EventMeshKafka";
            GroupId = "EventMeshKafka";
            GetMetadataTimeout = TimeSpan.FromSeconds(2);
            ListenKafkaTopicTimerMS = 5 * 1000;
        }

        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public string JobId { get; set; }
        public TimeSpan GetMetadataTimeout { get; set; }
        public int ListenKafkaTopicTimerMS { get; set; }
    }
}
