using FaasNet.EventMesh.Runtime.EF;
using FaasNet.EventMesh.Runtime.EF.Stores;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRuntimeEF(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> options = null)
        {
            var vpnStoreType = serviceCollection.FirstOrDefault(s => s.ServiceType == typeof(IVpnStore));
            var brokerConfigurationStoreType = serviceCollection.FirstOrDefault(s => s.ServiceType == typeof(IBrokerConfigurationStore));
            if (vpnStoreType != null)
                serviceCollection.Remove(vpnStoreType);
            if (brokerConfigurationStoreType != null)
                serviceCollection.Remove(brokerConfigurationStoreType);
            serviceCollection.AddTransient<IVpnStore, EFVpnStore>();
            serviceCollection.AddTransient<IClientStore, EFClientStore>();
            serviceCollection.AddTransient<IMessageDefinitionRepository, EFMessageDefinitionRepository>();
            serviceCollection.AddTransient<IBrokerConfigurationStore, EFBrokerConfigurationStore>();
            serviceCollection.AddTransient<IApplicationDomainRepository, EFApplicationDomainRepository>();
            serviceCollection.AddDbContext<EventMeshDBContext>(options);
            return serviceCollection;
        }
    }
}
