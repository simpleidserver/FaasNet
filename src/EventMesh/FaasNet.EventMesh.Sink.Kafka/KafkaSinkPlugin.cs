using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Sink.Kafka
{
    public class KafkaSinkPlugin : ISinkPlugin<KafkaSinkOptions>
    {
        public void Load(IServiceCollection services, SinkOptions sinkOptions, KafkaSinkOptions pluginOptions)
        {
            services.AddKafkaSeed(s =>
            {
                s.EventMeshPort = sinkOptions.EventMeshPort;
                s.EventMeshUrl = sinkOptions.EventMeshUrl;
                s.Vpn = sinkOptions.Vpn;
                s.ClientId = sinkOptions.ClientId;
            }, s =>
            {
                s.BootstrapServers = pluginOptions.BootstrapServers;
                s.GroupId = pluginOptions.GroupId;
                s.JobId = pluginOptions.JobId;
                s.GetMetadataTimeout = pluginOptions.GetMetadataTimeout;
                s.ListenKafkaTopicTimerMS = pluginOptions.ListenKafkaTopicTimerMS;
            });
        }
    }
}
