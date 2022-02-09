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

        public static Package Disconnect()
        {
            var result = new Package
            {
                Header = new Header(Commands.DISCONNECT_REQUEST, HeaderStatus.SUCCESS, GenerateRandomSeq())
            };
            return result;
        }

        public static SubscriptionRequest Subscribe(ICollection<SubscriptionItem> subscriptionItems, string seq = null)
        {
            var result = new SubscriptionRequest
            {
                Header = new Header(Commands.SUBSCRIBE_REQUEST, HeaderStatus.SUCCESS, seq ?? GenerateRandomSeq()),
                Topics = subscriptionItems
            };
            return result;
        }

        public static Package AsyncMessageAckToServer(string brokerName, string topic, int nbCloudEventsConsumed, string seq)
        {
            var result = new AsyncMessageAckToServer
            {
                Header = new Header(Commands.ASYNC_MESSAGE_TO_CLIENT_ACK, HeaderStatus.SUCCESS, seq),
                BrokerName = brokerName,
                Topic = topic,
                NbCloudEventsConsumed = nbCloudEventsConsumed
            };
            return result;
        }

        public static Package AddBridge(string urn, int port, string seq = null)
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
