using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventMesh.Runtime
{
    public static class EventMeshRuntimeHostBuilderExtensions
    {
        public static RuntimeHostBuilder AddEF(this RuntimeHostBuilder eventMeshRuntime, Action<DbContextOptionsBuilder> options = null)
        {
            eventMeshRuntime.ServiceCollection.AddRuntimeEF(options);
            return eventMeshRuntime;
        }
    }
}
