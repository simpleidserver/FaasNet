using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Sink
{
    public interface ISinkPlugin<T> where T : class
    {
        void Load(IServiceCollection services, SinkOptions sinkOptions, T pluginOptions);
    }
}
