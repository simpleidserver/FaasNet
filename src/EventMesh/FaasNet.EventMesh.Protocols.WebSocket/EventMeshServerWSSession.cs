using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client;
using NetCoreServer;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Protocols.WebSocket
{
    public class EventMeshServerWSSession: WsSession
    {
        private readonly EventMeshWebSocketOptions _options;

        public EventMeshServerWSSession(EventMeshWebSocketOptions options, WsServer server) : base(server)
        {
            _options = options;
        }

        public override void OnWsConnected(HttpRequest request)
        {
            base.OnWsConnected(request);
        }

        public override void OnWsDisconnected()
        {
            base.OnWsDisconnected();
        }

        public override async void OnWsReceived(byte[] buffer, long offset, long size)
        {
            var json = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            if (await TryPublishMessage(json)) return;
            if (await TryDirectSubscribe(json)) return;
        }

        private async Task<bool> TryPublishMessage(string json)
        {
            var publishMessageRequest = JsonSerializer.Deserialize<PublishMessageRequest>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (publishMessageRequest == null || publishMessageRequest.RequestType != "PUBLISH") return false;
            var cloudEvent = new CloudEvent
            {
                Type = publishMessageRequest.Topic,
                Source = new Uri("https://eventmeshserver.com"),
                Subject = publishMessageRequest.Subject,
                Id = publishMessageRequest.Id,
                Time = DateTimeOffset.Now,
                DataContentType = "application/json",
                Data = publishMessageRequest.Content
            };
            var eventMeshClient = new EventMeshClient(_options.EventMeshUrl, _options.EventMeshPort);
            var pubSession = await eventMeshClient.CreatePubSession(publishMessageRequest.Vpn, publishMessageRequest.ClientId, null, CancellationToken.None);
            await pubSession.Publish(publishMessageRequest.Topic, cloudEvent, CancellationToken.None);
            return true;
        }

        private async Task<bool> TryDirectSubscribe(string json)
        {
            var subscribeRequest = JsonSerializer.Deserialize<DirectSubscribeRequest>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (subscribeRequest == null || subscribeRequest.RequestType != "DIRECTLY_SUBSCRIBE") return false;
            var eventMeshClient = new EventMeshClient(_options.EventMeshUrl, _options.EventMeshPort);
            var subSession = await eventMeshClient.CreateSubSession(subscribeRequest.Vpn, subscribeRequest.ClientId, null, CancellationToken.None);
            subSession.DirectSubscribe(subscribeRequest.Filter, (ce) =>
            {
                var json = JsonSerializer.Serialize(new CloudEventResult { Data = ce.Data.ToString(), Type = ce.Type });
                var session = SendText(json);
            }, CancellationToken.None);
            return true;
        }

        private class PublishMessageRequest
        {
            public string RequestType { get; set; }
            public string Vpn { get; set; }
            public string ClientId { get; set; }
            public string Topic { get; set; }
            public string Id { get; set; }
            public string Subject { get; set; }
            public string Content { get; set; }
        }

        private class DirectSubscribeRequest
        {
            public string RequestType { get; set; }
            public string Vpn { get; set; }
            public string ClientId { get; set; }
            public string Filter { get; set; }
        }

        private class CloudEventResult
        {
            public string Type { get; set; }
            public string Data { get; set; }
        }
    }
}
