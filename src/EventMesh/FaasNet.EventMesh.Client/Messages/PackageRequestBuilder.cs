﻿using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
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

        public static BaseEventMeshPackage GetAllVpn(FilterQuery filter)
        {
            return new GetAllVpnRequest(GenerateRandomSeq())
            {
                Filter = filter
            };
        }

        public static BaseEventMeshPackage AddClient(string clientId, string vpn, ICollection<ClientPurposeTypes> purposes)
        {
            return new AddClientRequest(GenerateRandomSeq(), clientId, vpn, purposes);
        }

        public static BaseEventMeshPackage GetAllClient(FilterQuery filter)
        {
            return new GetAllClientRequest(GenerateRandomSeq())
            {
                Filter = filter
            };
        }

        public static BaseEventMeshPackage AddQueue(string vpn, string name, string topicFilter)
        {
            return new AddQueueRequest(GenerateRandomSeq())
            {
                Vpn = vpn,
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

        public static BaseEventMeshPackage Hello(string clientId, string vpn, string clientSecret, string queueName, ClientPurposeTypes purpose)
        {
            return new HelloRequest(GenerateRandomSeq())
            {
                ClientId = clientId,
                Vpn = vpn,
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

        public static BaseEventMeshPackage GetClient(string clientId, string vpn)
        {
            return new GetClientRequest(GenerateRandomSeq())
            {
                ClientId = clientId,
                Vpn = vpn
            };
        }

        public static BaseEventMeshPackage SearchSessions(string clientId, string vpn, FilterQuery filter)
        {
            return new SearchSessionsRequest(GenerateRandomSeq())
            {
                ClientId = clientId,
                Vpn = vpn,
                Filter = filter
            };
        }

        public static BaseEventMeshPackage SearchQueues(FilterQuery filter)
        {
            return new SearchQueuesRequest(GenerateRandomSeq())
            {
                Filter = filter
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
