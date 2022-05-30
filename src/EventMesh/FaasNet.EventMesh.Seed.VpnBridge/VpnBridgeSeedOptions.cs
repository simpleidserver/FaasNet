namespace FaasNet.EventMesh.Seed.VpnBridge
{
    public class VpnBridgeSeedOptions
    {
        public VpnBridgeSeedOptions()
        {
            JobId = "VpnBridge";
            GetBridgeServerLstIntervalMS = 5 * 1000;
            EventMeshServerGroupId = "VpnBridgeGroupId";
        }

        public string JobId { get; set; }
        /// <summary>
        /// Interval MS - get list of bridge servers.
        /// </summary>
        public int GetBridgeServerLstIntervalMS { get; set; }
        /// <summary>
        /// Group identifier used to subscribe to a topic.
        /// </summary>
        public string EventMeshServerGroupId { get; set; }
    }
}
