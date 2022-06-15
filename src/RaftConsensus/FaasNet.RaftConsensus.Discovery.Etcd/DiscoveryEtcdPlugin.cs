using FaasNet.EventMesh.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.RaftConsensus.Discovery.Etcd
{
    public class DiscoveryEtcdPlugin : IPlugin<EtcdOptions>
    {
        public void Load(IServiceCollection services, EtcdOptions options)
        {
            services.AddEtcdDiscovery(o =>
            {
                o.EventMeshPrefix = options.EventMeshPrefix;
                o.Username = options.Username;
                o.Password = options.Password;
                o.ConnectionString = options.ConnectionString;
            });
        }
    }
}
