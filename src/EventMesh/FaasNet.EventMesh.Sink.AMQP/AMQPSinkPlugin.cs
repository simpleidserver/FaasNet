﻿using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Sink.AMQP
{
    public class AMQPSinkPlugin : ISinkPlugin<EventMeshSinkAMQPOptions>
    {
        public void Load(IServiceCollection services, SinkOptions sinkOptions, EventMeshSinkAMQPOptions pluginOptions)
        {
            services.AddAMQPSeed(s =>
            {
                s.EventMeshPort = sinkOptions.EventMeshPort;
                s.EventMeshUrl = sinkOptions.EventMeshUrl;
                s.Vpn = sinkOptions.Vpn;
                s.ClientId = sinkOptions.ClientId;
            }, s =>
            {
                s.JobId = pluginOptions.JobId;
                s.ConnectionFactory = pluginOptions.ConnectionFactory;
                s.TopicName = pluginOptions.TopicName;
            });
        }
    }
}
