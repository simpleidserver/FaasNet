namespace FaasNet.RaftConsensus.Client.Messages
{
    public static class GossipPackageRequestBuilder
    {
        public static GossipPackage Heartbeat(string entityType, int entityVersion)
        {
            return new GossipHeartbeatRequest
            {
                Header = new GossipHeader(GossipCommands.HEARTBEAT_REQUEST),
                EntityType = entityType,
                EntityVersion = entityVersion
            };
        }
    }
}
