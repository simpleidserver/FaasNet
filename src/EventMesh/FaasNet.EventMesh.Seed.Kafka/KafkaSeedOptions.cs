using System;

namespace FaasNet.EventMesh.Seed.Kafka
{
    public class KafkaSeedOptions
    {
        public KafkaSeedOptions()
        {
            JobId = "EventMeshKafka";
            GetMetadataTimeout = TimeSpan.FromSeconds(2);
        }

        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public string JobId { get; set; }
        public TimeSpan GetMetadataTimeout { get; set; }
    }
}
