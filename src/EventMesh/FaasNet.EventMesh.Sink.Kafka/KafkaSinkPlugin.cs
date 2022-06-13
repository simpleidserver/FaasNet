using FaasNet.EventMesh.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Sink.Kafka
{
    public class KafkaSinkPlugin : IPlugin<KafkaSinkOptions>
    {
        public void Load(IServiceCollection services, KafkaSinkOptions pluginOptions)
        {
            services.AddKafkaSeed(s =>
            {
                s.EventMeshPort = pluginOptions.EventMeshPort;
                s.EventMeshUrl = pluginOptions.EventMeshUrl;
                s.Vpn = pluginOptions.Vpn;
                s.ClientId = pluginOptions.ClientId;
                s.BootstrapServers = pluginOptions.BootstrapServers;
                s.GroupId = pluginOptions.GroupId;
                s.JobId = pluginOptions.JobId;
                s.GetMetadataTimeout = pluginOptions.GetMetadataTimeout;
                s.ListenKafkaTopicTimerMS = pluginOptions.ListenKafkaTopicTimerMS;
            });
        }
    }
}
