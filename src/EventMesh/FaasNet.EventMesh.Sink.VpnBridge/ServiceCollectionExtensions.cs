using FaasNet.EventMesh.Sink;
using FaasNet.EventMesh.Sink.VpnBridge;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVpnBridgeSeed(this IServiceCollection services, Action<VpnBridgeSinkOptions> vpnBridgeSeedOptions = null)
        {
            if (vpnBridgeSeedOptions == null)
            {
                services.Configure<VpnBridgeSinkOptions>((o) => { });
            }
            else
            {
                var opt = new VpnBridgeSinkOptions();
                vpnBridgeSeedOptions(opt);
                services.AddSeed((t) =>
                {
                    t.EventMeshPort = opt.EventMeshPort;
                    t.EventMeshUrl = opt.EventMeshUrl;
                    t.Vpn = opt.Vpn;
                    t.ClientId = opt.ClientId;
                });
                services.Configure(vpnBridgeSeedOptions);
            }

            services.AddTransient<ISinkJob, VpnBridgeSinkJob>();
            return services;
        }
    }
}
