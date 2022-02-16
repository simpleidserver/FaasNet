using EventMesh.Runtime.Models;
using System.Collections.Generic;

namespace EventMesh.Runtime.Kafka
{
    public static class BrokerConfigurationExtensions
    {
        public static KafkaOptions ToKafkaOptions(this BrokerConfiguration conf)
        {
            return new KafkaOptions
            {
                BrokerName = conf.Name,
                BootstrapServers = conf.GetValue("BootstrapServers")
            };
        }

        public static BrokerConfiguration ToConfiguration(this KafkaOptions opts)
        {
            return new BrokerConfiguration
            {
                Name = opts.BrokerName,
                Protocol = Constants.Protocol,
                Records = new List<BrokerConfigurationRecord>
                {
                    new BrokerConfigurationRecord
                    {
                        Key = "BootstrapServers",
                        Value = opts.BootstrapServers
                    }
                }
            };
        }
    }
}
