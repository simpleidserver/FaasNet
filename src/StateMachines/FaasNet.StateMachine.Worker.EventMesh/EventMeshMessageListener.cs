using FaasNet.EventMesh.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker.EventMesh
{
    public class EventMeshMessageListener : IMessageListener
    {
        public const string NAME = "EventMesh";
        private EventMeshOptions _options;
        private readonly ICollection<MessageBrokerListener> _listeners;

        public string Name => NAME;

        public EventMeshMessageListener(IOptions<EventMeshOptions> options)
        {
            _options = options.Value;
            _listeners = new List<MessageBrokerListener>();
        }

        public async Task<IMessageListenerResult> Listen(Func<MessageResult, Task> callback, CancellationToken cancellationToken)
        {
            var vpns = await GetAllVpns(cancellationToken);
            foreach(var vpn in vpns)
            {
                var evtMeshClient = new EventMeshClient(_options.ClientId, _options.Password, vpn, _options.Url, _options.Port);
                var subscriptionResult = await evtMeshClient.Subscribe("*", async (msg) =>
                {
                    var msgResult = new MessageResult
                    {
                        Vpn = vpn,
                        Content = msg.CloudEvents,
                        TopicMessage = msg.TopicMessage
                    };
                    await callback(msgResult);
                }, cancellationToken);
                _listeners.Add(new MessageBrokerListener { EvtMeshClient = evtMeshClient, SubscriptionResult = subscriptionResult });
            }

            return new EventMeshMessageListenerResult(Dispose);
        }

        public void Dispose()
        {
            foreach(var listener in _listeners)
            {
                listener.SubscriptionResult.Stop();
            }
        }

        private async Task<IEnumerable<string>> GetAllVpns(CancellationToken cancellationToken)
        {
            using (var evtMeshClient = new EventMeshClient(_options.ClientId, _options.Password))
            {
                return await evtMeshClient.GetAllVpns(cancellationToken);
            }
        }

        public class EventMeshMessageListenerResult : IMessageListenerResult
        {
            private readonly Action _callback;

            public EventMeshMessageListenerResult(Action callback)
            {
                _callback = callback;
            }

            public void Stop()
            {
                _callback();
            }
        }

        public class MessageBrokerListener
        {
            public EventMeshClient EvtMeshClient { get; set; }
            public SubscriptionResult SubscriptionResult { get; set; }
        }
    }
}
