using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.Common
{
    public class ServerBuilder
    {
        public ServerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; private set; }
    }
}
