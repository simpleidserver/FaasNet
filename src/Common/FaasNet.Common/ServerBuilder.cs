using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.Common
{
    public class ServerBuilder
    {
        private ServiceProvider _serviceProvider;

        public ServerBuilder()
        {
            Services = new ServiceCollection();
        }

        public ServerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider
        {
            get
            {
                if(_serviceProvider == null) _serviceProvider = Services.BuildServiceProvider();
                return _serviceProvider;
            }
        }
    }
}
