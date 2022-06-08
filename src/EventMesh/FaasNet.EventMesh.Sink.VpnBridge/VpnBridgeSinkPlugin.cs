using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Sink.VpnBridge
{
    public class VpnBridgeSinkPlugin : ISinkPlugin<VpnBridgeSinkOptions>
    {
        public void Load(IServiceCollection services, SinkOptions sinkOptions, VpnBridgeSinkOptions pluginOptions)
        {
            services.AddVpnBridgeSeed(s =>
            {
                s.EventMeshPort = sinkOptions.EventMeshPort;
                s.EventMeshUrl = sinkOptions.EventMeshUrl;
                s.Vpn = sinkOptions.Vpn;
                s.ClientId = sinkOptions.ClientId;
            }, s =>
            {
                s.JobId = pluginOptions.JobId;
                s.GetBridgeServerLstIntervalMS = pluginOptions.GetBridgeServerLstIntervalMS;
                s.EventMeshServerGroupId = pluginOptions.EventMeshServerGroupId;
            });
        }
    }
}
