﻿using CloudNative.CloudEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaasNet.EventMesh.Client.Messages
{
    public static class PackageRequestBuilder
    {
        private const int SEQ_LENGTH = 10;

        public static Package HeartBeat()
        {
            var result = new Package
            {
                Header = new Header(Commands.HEARTBEAT_REQUEST, HeaderStatus.SUCCESS, GenerateRandomSeq())
            };
            return result;
        }

        public static Package AddVPN(string vpn)
        {
            var result = new AddVpnRequest
            {
                Header = new Header(Commands.ADD_VPN_REQUEST, HeaderStatus.SUCCESS, GenerateRandomSeq()),
                Vpn = vpn
            };
            return result;
        }

        public static Package AddClient(string vpn, string clientId, ICollection<UserAgentPurpose> purposes)
        {
            var result = new AddClientRequest
            {
                Header = new Header(Commands.ADD_CLIENT_REQUEST, HeaderStatus.SUCCESS, GenerateRandomSeq()),
                Vpn = vpn,
                ClientId = clientId,
                Purposes = purposes
            };
            return result;
        }

        public static HelloRequest Hello(UserAgent userAgent)
        {
            var result = new HelloRequest
            {
                Header = new Header(Commands.HELLO_REQUEST, HeaderStatus.SUCCESS, GenerateRandomSeq()),
                UserAgent = userAgent
            };
            return result;
        }

        public static GetAllVpnRequest GetAllVpns()
        {
            var result = new GetAllVpnRequest
            {
                Header = new Header(Commands.GET_ALL_VPNS_REQUEST, HeaderStatus.SUCCESS, GenerateRandomSeq())
            };
            return result;
        }

        public static DisconnectRequest Disconnect(string clientId, string sessionId)
        {
            var result = new DisconnectRequest
            {
                Header = new Header(Commands.DISCONNECT_REQUEST, HeaderStatus.SUCCESS, GenerateRandomSeq()),
                ClientId = clientId,
                SessionId = sessionId
            };
            return result;
        }

        public static SubscriptionRequest Subscribe(string groupId, ICollection<SubscriptionItem> subscriptionItems, string sessionId, string seq = null)
        {
            var result = new SubscriptionRequest
            {
                Header = new Header(Commands.SUBSCRIBE_REQUEST, HeaderStatus.SUCCESS, seq ?? GenerateRandomSeq()),
                GroupId = groupId,
                TopicFilters = subscriptionItems,
                SessionId = sessionId
            };
            return result;
        }

        public static Package PublishMessage(string sessionId, string topicName, CloudEvent cloudEvent, string seq = null)
        {
            var result = new PublishMessageRequest
            {
                Header = new Header(Commands.PUBLISH_MESSAGE_REQUEST, HeaderStatus.SUCCESS, seq ?? GenerateRandomSeq()),
                CloudEvent = cloudEvent,
                SessionId = sessionId,
                Topic = topicName
            };
            return result;
        }

        public static Package AddBridge(string vpn, string urn, int port, string targetVpn, string seq = null)
        {
            var result = new AddBridgeRequest
            {
                Header = new Header(Commands.ADD_BRIDGE_REQUEST, HeaderStatus.SUCCESS, seq ?? GenerateRandomSeq()),
                TargetPort = port,
                TargetUrn = urn,
                TargetVpn = targetVpn,
                Vpn = vpn
            };
            return result;
        }

        public static Package ReadNextMessage(string sessionId, string groupId, string seq = null)
        {
            var result = new ReadNextMessageRequest
            {
                Header = new Header(Commands.READ_NEXT_MESSAGE_REQUEST, HeaderStatus.SUCCESS, seq ?? GenerateRandomSeq()),
                SessionId = sessionId,
                GroupId = groupId
            };
            return result;
        }

        public static Package ReadTopicMessage(string topic, int evtOffset, string seq = null)
        {
            var result = new ReadMessageTopicRequest
            {
                Header = new Header(Commands.READ_TOPIC_MESSAGE_REQUEST, HeaderStatus.SUCCESS, seq ?? GenerateRandomSeq()),
                Topic = topic,
                EvtOffset = evtOffset
            };
            return result;
        }

        private static string GenerateRandomSeq()
        {
            var builder = new StringBuilder(SEQ_LENGTH);
            var rnd = new Random();
            for(int i = 0; i < SEQ_LENGTH; i++)
            {
                builder.Append((char)rnd.Next(48, 57));
            }

            return builder.ToString();
        }
    }
}
