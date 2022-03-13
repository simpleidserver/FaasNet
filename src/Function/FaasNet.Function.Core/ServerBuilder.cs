using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.Function.Core
{
    public class ServerBuilder
    {
        internal ServerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
