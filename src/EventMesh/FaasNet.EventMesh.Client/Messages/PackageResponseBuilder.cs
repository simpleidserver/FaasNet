using CloudNative.CloudEvents;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.Messages
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

        public static Package Hello(string seq, string sessionId)
        {
            var result = new HelloResponse
            {
                Header = new Header(Commands.HELLO_RESPONSE, HeaderStatus.SUCCESS, seq),
                SessionId = sessionId
            };
            return result;
        }

        public static Package GetAllVpns(string seq, IEnumerable<string> vpns)
        {
            var result = new GetAllVpnResponse
            {
                Header = new Header(Commands.GET_ALL_VPNS_RESPONSE, HeaderStatus.SUCCESS, seq),
                Vpns = vpns
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

        public static Package AsyncMessageToClient(ICollection<AsyncMessageBridgeServer> bridgeServers, string brokerName, string topicName, string topicFilter, ICollection<CloudEvent> cloudEvts)
        {
            var result = new AsyncMessageToClient
            {
                Header = new Header(Commands.ASYNC_MESSAGE_TO_CLIENT, HeaderStatus.SUCCESS, null),
                BridgeServers = bridgeServers,
                BrokerName = brokerName,
                TopicMessage = topicName,
                CloudEvents = cloudEvts,
                TopicFilter = topicFilter
            };
            return result;
        }

        public static Package AsyncMessageToServer(string clientId, ICollection<AsyncMessageBridgeServer> bridgeServers, string brokerName, string topicMessage, string topicFilter, ICollection<CloudEvent> cloudEvts, string sessionId)
        {
            var result = new AsyncMessageToServer
            {
                Header = new Header(Commands.ASYNC_MESSAGE_TO_SERVER, HeaderStatus.SUCCESS, null),
                ClientId = clientId,
                BridgeServers = bridgeServers,
                BrokerName = brokerName,
                TopicMessage = topicMessage,
                CloudEvents = cloudEvts,
                SessionId = sessionId,
                TopicFilter = topicFilter
            };
            return result;
        }

        public static Package PublishMessage(string seq)
        {
            var result = new Package
            {
                Header = new Header(Commands.PUBLISH_MESSAGE_RESONSE, HeaderStatus.SUCCESS, seq)
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

        public static Package AsyncMessageToClient(string seq)
        {
            var result = new Package
            {
                Header = new Header(Commands.ASYNC_MESSAGE_TO_CLIENT_ACK_RESPONSE, HeaderStatus.SUCCESS, seq)
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
