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
    }
}
