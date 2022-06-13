using FaasNet.EventMesh.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Sink.VpnBridge
{
    public class VpnBridgeSinkPlugin : IPlugin<VpnBridgeSinkOptions>
    {
        public void Load(IServiceCollection services, VpnBridgeSinkOptions pluginOptions)
        {
            services.AddVpnBridgeSeed(s =>
            {
                s.EventMeshPort = pluginOptions.EventMeshPort;
                s.EventMeshUrl = pluginOptions.EventMeshUrl;
                s.Vpn = pluginOptions.Vpn;
                s.ClientId = pluginOptions.ClientId;
                s.JobId = pluginOptions.JobId;
                s.GetBridgeServerLstIntervalMS = pluginOptions.GetBridgeServerLstIntervalMS;
                s.EventMeshServerGroupId = pluginOptions.EventMeshServerGroupId;
            });
        }
    }
}
