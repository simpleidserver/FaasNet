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

        public static Package Client(string seq)
        {
            var result = new AddClientResponse
            {
                Header = new Header(Commands.ADD_CLIENT_RESPONSE, HeaderStatus.SUCCESS, seq)
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

        public static Package Subscription(string seq, string queueName)
        {
            var result = new SubscriptionResult
            {
                Header = new Header(Commands.SUBSCRIBE_RESPONSE, HeaderStatus.SUCCESS, seq),
                QueueName = queueName
            };
            return result;
        }

        public static Package Subscription(string seq)
        {
            var result = new SubscriptionResult
            {
                Header = new Header(Commands.SUBSCRIBE_RESPONSE, HeaderStatus.SUCCESS, seq),
                QueueName = string.Empty
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

        public static Package PublishMessage(string seq)
        {
            var result = new Package
            {
                Header = new Header(Commands.PUBLISH_MESSAGE_RESONSE, HeaderStatus.SUCCESS, seq)
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

        public static Package AddVpn(string seq)
        {
            var result = new Package
            {
                Header = new Header(Commands.ADD_VPN_RESPONSE, HeaderStatus.SUCCESS, seq)
            };
            return result;
        }

        public static Package GetBridges(IEnumerable<BridgeServerResponse> servers, string seq)
        {
            var result = new GetAllBridgeResponse
            {
                Header = new Header(Commands.GET_ALL_BRIDGE_VPN_RESPONSE, HeaderStatus.SUCCESS, seq),
                Servers = servers
            };
            return result;
        }

        public static Package ReadNextMessage(CloudEvent cloudEvt, string seq)
        {
            return new ReadNextMessageResponse
            {
                Header = new Header(Commands.READ_NEXT_MESSAGE_RESPONSE, HeaderStatus.SUCCESS, seq),
                ContainsMessage = cloudEvt != null,
                CloudEvt = cloudEvt
            };
        }

        public static Package ReadTopicMessage(string topic, CloudEvent value, int evtOffset, string seq)
        {
            return new ReadMessageTopicResponse
            {
                Header = new Header(Commands.READ_TOPIC_MESSAGE_RESPONSE, HeaderStatus.SUCCESS, seq),
                Topic = topic,
                Value = value,
                EvtOffset = evtOffset,
                ContainsMessage = value != null
            };
        }

        public static Package GetAllPlugins(ICollection<PluginResponse> plugins, string seq)
        {
            return new GetAllPluginsResponse
            {
                Header = new Header(Commands.GET_ALL_PLUGINS_RESPONSE, HeaderStatus.SUCCESS, seq),
                Plugins = plugins
            };
        }

        public static Package EnablePlugin(string seq)
        {
            return new Package
            {
                Header = new Header(Commands.ENABLE_PLUGIN_RESPONSE, HeaderStatus.SUCCESS, seq)
            };
        }

        public static Package DisablePlugin(string seq)
        {
            return new Package
            {
                Header = new Header(Commands.DISABLE_PLUGIN_RESPONSE, HeaderStatus.SUCCESS, seq)
            };
        }

        public static Package GetPluginConfiguration(ICollection<PluginConfigurationRecordResponse> records, string seq)
        {
            return new GetPluginConfigurationResponse
            {
                Header = new Header(Commands.GET_PLUGIN_CONFIGURATION_RESPONSE, HeaderStatus.SUCCESS, seq),
                Records = records
            };
        }

        public static Package UpdatePluginConfiguration(string seq)
        {
            return new GetPluginConfigurationResponse
            {
                Header = new Header(Commands.UPDATE_PLUGIN_CONFIGURATION_RESPONSE, HeaderStatus.SUCCESS, seq)
            };
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
