using System;
using System.Collections.Generic;
using System.Text;

namespace EventMesh.Runtime.Messages
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

        public static HelloRequest Hello(UserAgent userAgent)
        {
            var result = new HelloRequest
            {
                Header = new Header(Commands.HELLO_REQUEST, HeaderStatus.SUCCESS, GenerateRandomSeq()),
                UserAgent = userAgent
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

        public static SubscriptionRequest Subscribe(string clientId, ICollection<SubscriptionItem> subscriptionItems, string sessionId, string seq = null)
        {
            var result = new SubscriptionRequest
            {
                Header = new Header(Commands.SUBSCRIBE_REQUEST, HeaderStatus.SUCCESS, seq ?? GenerateRandomSeq()),
                Topics = subscriptionItems,
                ClientId = clientId,
                SessionId = sessionId
            };
            return result;
        }

        public static Package AsyncMessageAckToServer(string clientId, string brokerName, string topic, int nbCloudEventsConsumed, ICollection<AsyncMessageBridgeServer> bridgeServers, string sessionId, string seq = null)
        {
            var result = new AsyncMessageAckToServer
            {
                Header = new Header(Commands.ASYNC_MESSAGE_TO_CLIENT_ACK, HeaderStatus.SUCCESS, seq ?? GenerateRandomSeq()),
                BrokerName = brokerName,
                Topic = topic,
                NbCloudEventsConsumed = nbCloudEventsConsumed,
                BridgeServers = bridgeServers,
                ClientId = clientId,
                SessionId = sessionId
            };
            return result;
        }

        public static Package AddBridge(string urn, int port,string seq = null)
        {
            var result = new AddBridgeRequest
            {
                Header = new Header(Commands.ADD_BRIDGE_REQUEST, HeaderStatus.SUCCESS, seq ?? GenerateRandomSeq()),
                Port = port,
                Urn = urn
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
