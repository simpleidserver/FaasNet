﻿using RabbitMQ.Client;
using System;

namespace FaasNet.EventMesh.Seed.AMQP
{
    public class EventMeshSeedAMQPOptions
    {
        public EventMeshSeedAMQPOptions()
        {
            ConnectionFactory = (o) =>
            {
                o.HostName = "127.0.0.1";
                o.Port = 5672;
                o.UserName = "guest";
                o.Password = "guest";
            };
            TopicName = "amq.topic";
            JobId = "AMQPTopic";
        }

        public Action<ConnectionFactory> ConnectionFactory { get; set; }
        public string TopicName { get; set; }
        public string JobId { get; set; }
    }
}