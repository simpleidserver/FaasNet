using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.Application.Core
{
    public class AppServerBuilder
    {
        internal AppServerBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; set; }
    }
}
