using FaasNet.EventMesh.Plugin;

namespace FaasNet.EventMesh.Sink.VpnBridge
{
    public class VpnBridgeSinkOptions : SinkOptions
    {
        public VpnBridgeSinkOptions()
        {
            JobId = "VpnBridge";
            GetBridgeServerLstIntervalMS = 5 * 1000;
            EventMeshServerGroupId = "VpnBridgeGroupId";
        }

        /// <summary>
        /// Job identifier.
        /// </summary>
        [PluginEntryOptionProperty("jobId", "Job identifier.")]
        public string JobId { get; set; }
        /// <summary>
        /// Timer in MS to get bridge servers.
        /// </summary>
        [PluginEntryOptionProperty("bridgeTimerMS", "Timer in MS to get bridge servers.")]
        public int GetBridgeServerLstIntervalMS { get; set; }
        /// <summary>
        /// Group identifier used to subscribe to a topic.
        /// </summary>
        [PluginEntryOptionProperty("bridgeGroupId", "Group identifier used to subscribe to a topic.")]
        public string EventMeshServerGroupId { get; set; }
    }
}
