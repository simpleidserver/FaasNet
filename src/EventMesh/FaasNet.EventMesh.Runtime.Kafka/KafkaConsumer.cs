using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using FaasNet.EventMesh.Runtime.Events;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Kafka
{
    public class KafkaConsumer : BaseMessageConsumer<KafkaOptions>
    {
        private readonly List<KafkaSubscriptionRecord> _subscriptions = new List<KafkaSubscriptionRecord>();
        private readonly IBrokerConfigurationStore _brokerConfigurationStore;
        private readonly KafkaOptions _opts;
        private readonly IVpnStore _vpnStore;

        public KafkaConsumer(
            IBrokerConfigurationStore brokerConfigurationStore,
            IOptions<KafkaOptions> options,
            IVpnStore vpnStore,
            IOptions<RuntimeOptions> runtimeOpts) : base(runtimeOpts)
        {
            _brokerConfigurationStore = brokerConfigurationStore;
            _opts = options.Value;
            _vpnStore = vpnStore;
        }

        public override event EventHandler<CloudEventArgs> CloudEventReceived;

        public override string BrokerName
        {
            get
            {
                return _opts.BrokerName;
            }
        }

        public override Task Start(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override Task Stop(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void Commit(string topicName, Client client, string sessionId, int nbEvts)
        {
            var subscription = _subscriptions.First(s => s.TopicName == topicName && s.ClientSessionId == sessionId && s.ClientId == client.ClientId);
            subscription.Listener.Consume(nbEvts);
        }

        protected override void ListenTopic(KafkaOptions options, string topicName, ClientTopic topic, string clientId, string clientSessionId)
        {
            if (_subscriptions.Any(s => s.BrokerName == options.BrokerName && s.TopicName == topicName && s.ClientSessionId == clientSessionId && s.ClientId == clientId))
            {
                return;
            }

            var listener = new KafkaListener(options, topicName, clientId, clientSessionId, HandleMessage);
            listener.Start();
            _subscriptions.Add(new KafkaSubscriptionRecord(topicName, options.BrokerName, clientId, clientSessionId, listener));
        }

        protected override void UnsubscribeTopic(string topicName, Client client, string sessionId)
        {
            var subscription = _subscriptions.First(s => s.ClientSessionId == sessionId && s.ClientId == client.ClientId && s.TopicName == topicName);
            subscription.Listener.Stop();
            _subscriptions.Remove(subscription);
        }

        public override void Dispose()
        {
        }

        protected override KafkaOptions GetOptions()
        {
            return _brokerConfigurationStore.Get(_opts.BrokerName).ToKafkaOptions();
        }

        private void HandleMessage(string clientId, string clientSessionId, string topicName, ConsumeResult<string?, byte[]> message)
        {
            var jsonEventFormatter = new JsonEventFormatter();
            var cloudEvent = message.ToCloudEvent(jsonEventFormatter, "source", topicName);
            var vpn = _vpnStore.Get(clientId, clientSessionId, CancellationToken.None).Result;
            if (vpn == null)
            {
                return;
            }

            var client = vpn.GetClient(clientId);
            var brokerName = _opts.BrokerName;
            var clientSession = client.GetActiveSessionByTopic(brokerName, topicName);
            CloudEventReceived(this, new CloudEventArgs(topicName, brokerName, cloudEvent, client.ClientId, clientSession));
        }
    }
}
