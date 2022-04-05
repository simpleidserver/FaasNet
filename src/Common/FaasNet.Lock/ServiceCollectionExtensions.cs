using FaasNet.Lock;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLock(this IServiceCollection services)
        {
            return services.AddSingleton<IDistributedLock, InMemoryDistributedLock>();
        }
    }
}
