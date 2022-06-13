using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Plugin
{
    public interface IPlugin<T> where T : class
    {
        void Load(IServiceCollection services, T options);
    }
}
