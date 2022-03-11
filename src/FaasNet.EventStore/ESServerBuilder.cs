using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventStore
{
    public class ESServerBuilder
    {
        internal ESServerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; private set; }
    }
}
