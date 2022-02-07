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
            var clientStoreType = eventMeshRuntime.ServiceCollection.FirstOrDefault(s => s.ServiceType == typeof(IClientStore));
            eventMeshRuntime.ServiceCollection.Remove(clientStoreType);
            eventMeshRuntime.ServiceCollection.AddTransient<IClientStore, EFClientStore>();
            eventMeshRuntime.ServiceCollection.AddDbContext<EventMeshDBContext>(options);
            return eventMeshRuntime;
        }
    }
}
