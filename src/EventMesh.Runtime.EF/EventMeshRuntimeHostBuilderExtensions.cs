using EventMesh.Runtime.EF;
using EventMesh.Runtime.EF.Stores;
using EventMesh.Runtime.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace EventMesh.Runtime
{
    public static class EventMeshRuntimeHostBuilderExtensions
    {
        public static RuntimeHostBuilder AddEF(this RuntimeHostBuilder eventMeshRuntime, Action<DbContextOptionsBuilder> options = null)
        {
            var clientStoreType = eventMeshRuntime.ServiceCollection.First(s => s.ServiceType == typeof(IClientStore));
            var bridgeServerStoreType = eventMeshRuntime.ServiceCollection.First(s => s.ServiceType == typeof(IBridgeServerStore));
            eventMeshRuntime.ServiceCollection.Remove(clientStoreType);
            eventMeshRuntime.ServiceCollection.Remove(bridgeServerStoreType);
            eventMeshRuntime.ServiceCollection.AddTransient<IClientStore, EFClientStore>();
            eventMeshRuntime.ServiceCollection.AddTransient<IBridgeServerStore, EFBridgeServerStore>();
            eventMeshRuntime.ServiceCollection.AddDbContext<EventMeshDBContext>(options);
            return eventMeshRuntime;
        }
    }
}
