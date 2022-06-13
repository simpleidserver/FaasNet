using FaasNet.EventMesh.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Sink.AMQP
{
    public class AMQPSinkPlugin : IPlugin<EventMeshSinkAMQPOptions>
    {
        public void Load(IServiceCollection services, EventMeshSinkAMQPOptions pluginOptions)
        {
            services.AddAMQPSeed(s => 
            {
                s.EventMeshPort = pluginOptions.EventMeshPort;
                s.EventMeshUrl = pluginOptions.EventMeshUrl;
                s.Vpn = pluginOptions.Vpn;
                s.ClientId = pluginOptions.ClientId;
                s.JobId = pluginOptions.JobId;
                s.AMQPHostName = pluginOptions.AMQPHostName;
                s.AMQPPort = pluginOptions.AMQPPort;
                s.AMQPUserName = pluginOptions.AMQPUserName;
                s.AMQPPassword = pluginOptions.AMQPPassword;
                s.TopicName = pluginOptions.TopicName;
            });
        }
    }
}
