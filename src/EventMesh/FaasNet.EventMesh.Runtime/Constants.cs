namespace FaasNet.EventMesh.Runtime
{
    public static class Constants
    {
        public const string FilterWildcard = "*";
        public const string InMemoryBrokername = "inmemory";
        public const string ProtocolsPluginSubPath = "protocolPlugins";
        public const string SinksPluginSubPath = "sinkPlugins";
        public const string DiscoveriesPluginSubPath = "discoveryPlugins";

        public static class LockNames
        {
            public const string Client = "client-{0}";
        }
    }
}
