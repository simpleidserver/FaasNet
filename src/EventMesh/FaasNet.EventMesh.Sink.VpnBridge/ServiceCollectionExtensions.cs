using FaasNet.EventMesh.Sink;
using FaasNet.EventMesh.Sink.VpnBridge;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVpnBridgeSeed(this IServiceCollection services, Action<SinkOptions> seedOptionsCallback = null, Action<VpnBridgeSinkOptions> vpnBridgeSeedOptions = null)
        {
            services.AddSeed(seedOptionsCallback);
            if (vpnBridgeSeedOptions == null) services.Configure<VpnBridgeSinkOptions>((o) => { });
            else services.Configure(vpnBridgeSeedOptions);
            services.AddTransient<ISinkJob, VpnBridgeSinkJob>();
            return services;
        }
    }
}
