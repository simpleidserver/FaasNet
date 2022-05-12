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

        public static Package Client(string seq, string queue)
        {
            var result = new AddClientResponse
            {
                Header = new Header(Commands.ADD_CLIENT_RESPONSE, HeaderStatus.SUCCESS, seq),
                Queue = queue
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

        public static Package Subscription(string seq, IEnumerable<string> queueNames)
        {
            var result = new SubscriptionResult
            {
                Header = new Header(Commands.SUBSCRIBE_RESPONSE, HeaderStatus.SUCCESS, seq)
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

        public static Package AddVpn(string seq)
        {
            var result = new Package
            {
                Header = new Header(Commands.ADD_VPN_RESPONSE, HeaderStatus.SUCCESS, seq)
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
