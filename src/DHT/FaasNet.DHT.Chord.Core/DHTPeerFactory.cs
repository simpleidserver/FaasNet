using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.DHT.Chord.Core
{
    public interface IDHTPeerFactory
    {
        IDHTPeer Build();
    }

    public class DHTPeerFactory : IDHTPeerFactory
    {
        private readonly IServiceScopeFactory _serviceProvider;

        public DHTPeerFactory(IServiceScopeFactory serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDHTPeer Build()
        {
            var scope = _serviceProvider.CreateScope();
            return scope.ServiceProvider.GetService<IDHTPeer>();
        }
    }
}
