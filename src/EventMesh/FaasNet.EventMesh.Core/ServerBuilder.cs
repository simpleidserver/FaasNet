using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Core
{
    public class ServerBuilder
    {
        public ServerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
