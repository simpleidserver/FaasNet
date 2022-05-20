using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using FaasNet.EventMesh.Seed.Stores;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Seed.Kafka
{
    public class KafkaSeedJob : BaseSeedJob
    {
        private readonly KafkaSeedOptions _options;

        public KafkaSeedJob(ISubscriptionStore subscriptionStore, IOptions<SeedOptions> seedOptions, IOptions<KafkaSeedOptions> kafkaSeedOptions) : base(subscriptionStore, seedOptions)
        {
            _options = kafkaSeedOptions.Value;
        }

        protected override string JobId => _options.JobId;

        protected override Task Subscribe(CancellationToken cancellationToken)
        {
#pragma warning disable 4014
            Task.Run(async () => await InternalSubscribe(cancellationToken));
#pragma warning restore 4014
            return Task.CompletedTask;
        }

        protected override Task Unsubscribe(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task InternalSubscribe(CancellationToken cancellationToken)
        {
            var subscribedTopics = await GetAllSubscribedTopics(cancellationToken);
            var config = new ConsumerConfig
            {
                BootstrapServers = _options.BootstrapServers,
                GroupId = _options.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
            using (var consumer = new ConsumerBuilder<string, byte[]>(config).Build())
            {
                consumer.Assign(subscribedTopics);
                while(!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    var jsonEventFormatter = new JsonEventFormatter();
                    var cloudEvent = consumeResult.ToCloudEvent(jsonEventFormatter, "source", consumeResult.Topic);
                    await Publish(consumeResult.Topic, cloudEvent, cancellationToken);
                    await SubscriptionStore.IncrementOffset(JobId, consumeResult.Topic, cancellationToken);
                }
            }
        }

        private async Task<List<TopicPartitionOffset>> GetAllSubscribedTopics(CancellationToken cancellationToken)
        {
            var result = new List<TopicPartitionOffset>();
            var adminConfig = new AdminClientConfig
            {
                BootstrapServers = _options.BootstrapServers
            };
            using (var adminClient = new AdminClientBuilder(adminConfig).Build())
            {
                var metadata = adminClient.GetMetadata(_options.GetMetadataTimeout);
                var topicsMetadata = metadata.Topics;
                var topicNames = metadata.Topics.Select(t => t.Topic).ToList();
                foreach(var topicName in topicNames)
                {
                    var offset = await SubscriptionStore.GetOffset(JobId, topicName, cancellationToken);
                    result.Add(new TopicPartitionOffset(topicName, 0, new Offset(offset)));
                }

                return result;
            }
        }
    }
}
