using FaasNet.EventMesh.Seed;
using FaasNet.EventMesh.Seed.VpnBridge;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVpnBridgeSeed(this IServiceCollection services, Action<SeedOptions> seedOptionsCallback = null, Action<VpnBridgeSeedOptions> vpnBridgeSeedOptions = null)
        {
            services.AddSeed(seedOptionsCallback);
            if (vpnBridgeSeedOptions == null) services.Configure<VpnBridgeSeedOptions>((o) => { });
            else services.Configure(vpnBridgeSeedOptions);
            services.AddTransient<ISeedJob, VpnBridgeSeedJob>();
            return services;
        }
    }
}
