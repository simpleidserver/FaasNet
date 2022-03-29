using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Client
{
    public class EventMeshClient : IDisposable
    {
        private readonly string _clientId;
        private readonly string _password;
        private readonly string _vpn;
        private readonly RuntimeClient _runtimeClient;
        private readonly int _bufferCloudEvents;
        private HelloResponse _publishSession;
        private HelloResponse _subscribeSession;

        public EventMeshClient(string clientId, string password, string vpn = Constants.DefaultVpn, string url = Constants.DefaultUrl, int port = Constants.DefaultPort, int bufferCloudEvents = 1)
        {
            _clientId = clientId;
            _password = password;
            _vpn = vpn;
            _bufferCloudEvents = bufferCloudEvents;
            _runtimeClient = new RuntimeClient(url, port);
        }

        public async Task Connect( CancellationToken cancellationToken = default(CancellationToken))
        {
            await _runtimeClient.HeartBeat(cancellationToken);
        }

        public Task Publish(string topicName, object obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cloudEvt = new CloudEvent
            {
                Id = Guid.NewGuid().ToString(),
                Subject = topicName,
                Source = new Uri("http://localhost"),
                Type = topicName,
                DataContentType = "application/json",
                Data = JsonSerializer.Serialize(obj),
                Time = DateTimeOffset.UtcNow
            };
            return Publish(topicName, cloudEvt, cancellationToken);
        }

        public async Task Publish(string topicName, CloudEvent cloudEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_publishSession == null)
            {
                _publishSession = await CreateSession(_clientId, _password, UserAgentPurpose.PUB, cancellationToken);
            }

            await _runtimeClient.PublishMessage(_clientId, _publishSession.SessionId, topicName, cloudEvent);
        }

        public async Task<SubscriptionResult> SubscribeMessages<TMessage>(string topicName, Action<TMessage> callback, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class
        {
            return await Subscribe(topicName, (msg) =>
            {
                foreach(var cloudEvt in msg.CloudEvents)
                {
                    var deserialize = JsonSerializer.Deserialize(cloudEvt.Data.ToString(), typeof(TMessage)) as TMessage;
                    callback(deserialize);
                }
            }, cancellationToken);
        }

        public async Task<SubscriptionResult> Subscribe(string topicName, Action<AsyncMessageToClient> callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_subscribeSession == null)
            {
                _subscribeSession = await CreateSession(_clientId, _password, UserAgentPurpose.SUB, cancellationToken);
            }

            return await _runtimeClient.Subscribe(_clientId, _subscribeSession.SessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = topicName
                }
            }, callback, cancellationToken: cancellationToken);
        }

        public async Task Disconnect()
        {
            if (_publishSession != null)
            {
                await _runtimeClient.Disconnect(_clientId, _publishSession.SessionId);
            }

            if (_subscribeSession != null)
            {
                await _runtimeClient.Disconnect(_clientId, _subscribeSession.SessionId);
            }
        }

        public void Dispose()
        {
            Disconnect().Wait();
        }

        private Task<HelloResponse> CreateSession(string clientId, string password, UserAgentPurpose purpose, CancellationToken cancellationToken)
        {
            var processId = Process.GetCurrentProcess().Id;
            return _runtimeClient.Hello(new UserAgent
            {
                ClientId = clientId,
                Environment = "TST",
                Password = password,
                Pid = processId,
                Purpose = purpose,
                Version = "0",
                BufferCloudEvents = _bufferCloudEvents,
                Vpn = _vpn
            }, cancellationToken);
        }
    }
}
