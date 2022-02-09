using CloudNative.CloudEvents;
using System.Collections.Generic;

namespace EventMesh.Runtime.Messages
{
    public static class PackageResponseBuilder
    {
        public static Package HeartBeat(string seq)
        {
            var result = new Package
            {
                Header = new Header(Commands.HEARTBEAT_RESPONSE, HeaderStatus.SUCCESS, seq)
            };
            return result;
        }

        public static Package Hello(string seq)
        {
            var result = new Package
            {
                Header = new Header(Commands.HELLO_RESPONSE, HeaderStatus.SUCCESS, seq)
            };
            return result;
        }

        public static Package Subscription(string seq)
        {
            var result = new Package
            {
                Header = new Header(Commands.SUBSCRIBE_RESPONSE, HeaderStatus.SUCCESS, seq)
            };
            return result;
        }

        public static Package AsyncMessageToClient(string urn, string brokerName, string topicName, ICollection<CloudEvent> cloudEvts, string seq)
        {
            var result = new AsyncMessageToClient
            {
                Header = new Header(Commands.ASYNC_MESSAGE_TO_CLIENT, HeaderStatus.SUCCESS, seq),
                Urn = urn,
                BrokerName = brokerName,
                Topic = topicName,
                CloudEvents = cloudEvts,

            };
            return result;
        }

        public static Package AsyncMessageToServer(string clientId, string urn, string brokerName, string topicName, ICollection<CloudEvent> cloudEvts, string seq)
        {
            var result = new AsyncMessageToServer
            {
                Header = new Header(Commands.ASYNC_MESSAGE_TO_SERVER, HeaderStatus.SUCCESS, seq),
                ClientId = clientId,
                Urn = urn,
                BrokerName = brokerName,
                Topic = topicName,
                CloudEvents = cloudEvts
            };
            return result;
        }

        public static Package Bridge(string seq)
        {
            var result = new Package
            {
                // Header = new Header(Commands.BRIDGE_RESPONSE, HeaderStatus.SUCCESS, seq)
            };
            return result;
        }

        public static Package AddBridge(string seq)
        {
            var result = new Package
            {
                Header = new Header(Commands.ADD_BRIDGE_RESPONSE, HeaderStatus.SUCCESS, seq)
            };
            return result;
        }

        public static Package Disconnect(string seq)
        {
            var result = new Package
            {
                Header = new Header(Commands.DISCONNECT_RESPONSE, HeaderStatus.SUCCESS, seq)
            };
            return result;
        }

        public static Package Error(Commands requestCommand, string seq, Errors error)
        {
            var result = new Package
            {
                Header = new Header(Constants.MappingRequestToResponse[requestCommand], HeaderStatus.FAIL, seq, error)
            };
            return result;
        }
    }
}
