using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Kafka
{
    public class KafkaPublisher : IMessagePublisher
    {
        private readonly KafkaOptions _options;
        private readonly IBrokerConfigurationStore _brokerConfigurationStore;

        public KafkaPublisher(IOptions<KafkaOptions> options,
            IBrokerConfigurationStore brokerConfigurationStore)
        {
            _options = options.Value;
            _brokerConfigurationStore = brokerConfigurationStore;
        }

        public string BrokerName
        {
            get
            {
                return _options.BrokerName;
            }
        }

        public async Task Publish(CloudEvent cloudEvent, string topicName, Client client)
        {
            var options = GetOptions();
            var config = new ProducerConfig
            {
                BootstrapServers = options.BootstrapServers,
                ClientId = client.ClientId
            };
            var jsonEventFormatter = new JsonEventFormatter();
            using (var producer = new ProducerBuilder<string?, byte[]>(config).Build())
            {
                await producer.ProduceAsync(topicName, cloudEvent.ToKafkaMessage(jsonEventFormatter));
            }
        }

        private KafkaOptions GetOptions()
        {
            return _brokerConfigurationStore.Get(_options.BrokerName).ToKafkaOptions();
        }
    }
}
