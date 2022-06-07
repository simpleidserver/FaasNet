using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using FaasNet.EventMesh.Sink.Stores;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FaasNet.EventMesh.Sink.Kafka
{
    public class KafkaSinkJob : BaseSinkJob
    {
        private readonly KafkaSinkOptions _options;
        private ICollection<SubscriptionTopicRecord> _subscriptionTopics;
        private System.Timers.Timer _listenTopicTimer;
        private CancellationTokenSource _tokenSource;

        public KafkaSinkJob(ISubscriptionStore subscriptionStore, IOptions<SinkOptions> seedOptions, IOptions<KafkaSinkOptions> kafkaSeedOptions) : base(subscriptionStore, seedOptions)
        {
            _options = kafkaSeedOptions.Value;
        }

        protected override string JobId => _options.JobId;

        protected override Task Subscribe(CancellationToken cancellationToken)
        {
            _subscriptionTopics = new List<SubscriptionTopicRecord>();
            _listenTopicTimer = new System.Timers.Timer(_options.ListenKafkaTopicTimerMS);
            _listenTopicTimer.Elapsed += async (o, e) => await ListenTopic(o, e);
            _listenTopicTimer.AutoReset = false;
            _listenTopicTimer.Start();
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            return Task.CompletedTask;
        }

        protected override Task Unsubscribe(CancellationToken cancellationToken)
        {
            _listenTopicTimer.Stop();
            _tokenSource.Cancel();
            return Task.CompletedTask;
        }

        private async Task ListenTopic(object source, ElapsedEventArgs e)
        {
            var subscribedTopics = await GetAllSubscribedTopics(_tokenSource.Token);
            if (!subscribedTopics.Any())
            {
                _listenTopicTimer.Start();
                return;
            }

            var topicNotYetSubscribed = subscribedTopics.Where(st => !_subscriptionTopics.Any(t => t.HasPartition(st.Topic))).ToList();
            if(!topicNotYetSubscribed.Any())
            {
                _listenTopicTimer.Start();
                return;
            }


#pragma warning disable 4014
            Task.Run(async () => await ListenTopics(topicNotYetSubscribed));
#pragma warning restore 4014
            _subscriptionTopics.Add(new SubscriptionTopicRecord {  Partitions = topicNotYetSubscribed });
            _listenTopicTimer.Start();
        }

        private async Task ListenTopics(IEnumerable<TopicPartitionOffset> subscribedTopics)
        {
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
                while (!_tokenSource.Token.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(_tokenSource.Token);
                    var jsonEventFormatter = new JsonEventFormatter();
                    var cloudEvent = consumeResult.ToCloudEvent(jsonEventFormatter, "source", consumeResult.Topic);
                    await Publish(consumeResult.Topic, cloudEvent, _tokenSource.Token);
                    await SubscriptionStore.IncrementOffset(JobId, consumeResult.Topic, _tokenSource.Token);
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
                var topicNames = metadata.Topics.Select(t => t.Topic).Where(t => t != "__consumer_offsets_").ToList();
                foreach(var topicName in topicNames)
                {
                    var offset = await SubscriptionStore.GetOffset(JobId, topicName, cancellationToken);
                    result.Add(new TopicPartitionOffset(topicName, 0, new Offset(offset)));
                }

                return result;
            }
        }

        private class SubscriptionTopicRecord
        {
            public IEnumerable<TopicPartitionOffset> Partitions { get; set; }

            public bool HasPartition(string topic)
            {
                return Partitions.Any(p => p.Topic == topic);
            }
        }
    }
}
