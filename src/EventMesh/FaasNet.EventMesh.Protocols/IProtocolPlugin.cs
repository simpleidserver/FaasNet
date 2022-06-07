using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Protocols
{
    public interface IProtocolPlugin<T> where T : class
    {
        void Load(IServiceCollection services, T options);
    }
}
