using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.StateMachines;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaasNet.EventMesh.Client.Messages
{
    public static class PackageRequestBuilder
    {
        public static BaseEventMeshPackage HeartBeat()
        {
            return new PingRequest(GenerateRandomSeq());
        }
        public static BaseEventMeshPackage AddVpn(string vpn, string description)
        {
            return new AddVpnRequest(GenerateRandomSeq(), vpn, description);
        }

        public static BaseEventMeshPackage GetAllVpn()
        {
            return new GetAllVpnRequest(GenerateRandomSeq());
        }

        public static BaseEventMeshPackage AddClient(string clientId, string vpn, ICollection<ClientPurposeTypes> purposes)
        {
            return new AddClientRequest(GenerateRandomSeq(), clientId, vpn, purposes);
        }

        public static BaseEventMeshPackage GetAllClient()
        {
            return new GetAllClientRequest(GenerateRandomSeq());
        }

        public static BaseEventMeshPackage AddQueue(string name, string topicFilter)
        {
            return new AddQueueRequest(GenerateRandomSeq())
            {
                QueueName = name,
                TopicFilter = topicFilter
            };
        }

        public static BaseEventMeshPackage PublishMessage(string topic, string sessionId, CloudEvent cloudEvt)
        {
            return new PublishMessageRequest(GenerateRandomSeq())
            {
                Topic = topic,
                SessionId = sessionId,
                CloudEvent = cloudEvt
            };
        }

        public static BaseEventMeshPackage Hello(string clientId, string clientSecret, string queueName, ClientPurposeTypes purpose)
        {
            return new HelloRequest(GenerateRandomSeq())
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                QueueName = queueName,
                Purpose = purpose
            };
        }

        public static BaseEventMeshPackage ReadMessage(int offset, string sessionId)
        {
            return new ReadMessageRequest(GenerateRandomSeq())
            {
                Offset = offset,
                SessionId = sessionId
            };
        }

        private static string GenerateRandomSeq()
        {
            var builder = new StringBuilder(10);
            var rnd = new Random();
            for (int i = 0; i < 10; i++)
                builder.Append((char)rnd.Next(48, 57));
            return builder.ToString();
        }
    }
}
