using FaasNet.EventMesh.Client;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker.EventMesh
{
    public class EventMeshMessageListener : IMessageListener
    {
        public const string NAME = "EventMesh";
        private EventMeshOptions _options;

        public EventMeshMessageListener(IOptions<EventMeshOptions> options)
        {
            _options = options.Value;
        }

        public bool SupportVpn => true;
        public string Name => NAME;

        public async Task<IMessageListenerResult> Listen(string vpn, Action<MessageResult> callback, CancellationToken cancellationToken)
        {
            var evtMeshClient = new EventMeshClient(_options.ClientId, _options.Password, vpn, _options.Url, _options.Port);
            var result = await evtMeshClient.Subscribe("*", (msg) =>
            {
                var msgResult = new MessageResult
                {
                    Vpn = vpn,
                    Content = msg.CloudEvents
                };
                callback(msgResult);
            }, cancellationToken);
            return new MessageListenerResult(result);
        }

        public class MessageListenerResult : IMessageListenerResult
        {
            private readonly SubscriptionResult _subscriptionResult;

            public MessageListenerResult(SubscriptionResult subscriptionResult)
            {
                _subscriptionResult = subscriptionResult;
            }

            public void Stop()
            {
                _subscriptionResult.Stop();
            }
        }
    }
}
