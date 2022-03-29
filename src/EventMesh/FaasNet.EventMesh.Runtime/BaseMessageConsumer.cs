using FaasNet.EventMesh.Runtime.Events;
using FaasNet.EventMesh.Runtime.Models;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime
{
    public abstract class BaseMessageConsumer<TOptions> : IMessageConsumer where TOptions : BaseBrokerOptions
    {
        private RuntimeOptions _runtimeOpts;

        public BaseMessageConsumer(IOptions<RuntimeOptions> runtimeOpts)
        {
            _runtimeOpts = runtimeOpts.Value;
        }


        public abstract event EventHandler<CloudEventArgs> CloudEventReceived;

        public abstract string BrokerName { get; }

        public abstract Task Start(CancellationToken cancellationToken);

        public abstract Task Stop(CancellationToken cancellationToken);

        public abstract void Commit(string topicName, Models.Client client, string sessionId, int nbEvts);

        public async Task Subscribe(string topicName, Models.Client client, string sessionId, CancellationToken cancellationToken)
        {
            var options = await GetOptions(cancellationToken);
            var activeSession = client.GetActiveSession(sessionId);
            if (activeSession.HasTopic(topicName, options.BrokerName))
            {
                return;
            }

            var topic = client.GetTopic(topicName, options.BrokerName);
            if (topic == null)
            {
                topic = client.AddTopic(topicName, options.BrokerName);
            }

            Task.Run(() =>
            {
                Thread.Sleep(_runtimeOpts.WaitLocalSubscriptionIntervalMS);
                ListenTopic(options, topicName, topic, client.ClientId, sessionId);
            });

            activeSession.SubscribeTopic(topicName, options.BrokerName);
        }

        public async Task Unsubscribe(string topicName, Models.Client client, string sessionId, CancellationToken cancellationToken)
        {
            var options = await GetOptions(cancellationToken);
            var activeSession = client.GetActiveSession(sessionId);
            if (!activeSession.HasTopic(topicName, options.BrokerName))
            {
                return;
            }

            UnsubscribeTopic(topicName, client, sessionId);
        }

        protected abstract Task<TOptions> GetOptions(CancellationToken cancellationToken);

        protected abstract void ListenTopic(TOptions options, string topicName, ClientTopic topic, string clientId, string clientSessionId);

        protected abstract void UnsubscribeTopic(string topicName, Models.Client client, string sessionId);

        public abstract void Dispose();
    }
}
