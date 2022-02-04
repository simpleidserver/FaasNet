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

        public static Package AsyncMessageToClient(string brokerName, string topicName, ICollection<CloudEvent> cloudEvts, string seq)
        {
            var result = new AsyncMessageToClient
            {
                Header = new Header(Commands.ASYNC_MESSAGE_TO_CLIENT, HeaderStatus.SUCCESS, seq),
                BrokerName = brokerName,
                Topic = topicName,
                CloudEvents = cloudEvts
            };
            return result;
        }
    }
}
