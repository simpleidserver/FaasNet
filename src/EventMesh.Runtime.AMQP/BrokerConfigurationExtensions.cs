using EventMesh.Runtime.Models;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace EventMesh.Runtime.AMQP
{
    public static class BrokerConfigurationExtensions
    {
        public static AMQPOptions ToAMQPOptions(this BrokerConfiguration conf)
        {
            return new AMQPOptions
            {
                BrokerName = conf.Name,
                QueueName = conf.GetValue("queueName"),
                Source = conf.GetValue("source"),
                TopicName = conf.GetValue("topicName"),
                ConnectionFactory = (opt) =>
                {
                    opt.HostName = conf.GetValue("connectionFactory.hostName");
                    opt.Port = int.Parse(conf.GetValue("connectionFactory.port"));
                    opt.UserName = conf.GetValue("connectionFactory.username");
                    opt.Password = conf.GetValue("connectionFactory.password");
                }
            };
        }

        public static BrokerConfiguration ToConfiguration(this AMQPOptions opts)
        {
            var connectionFactory = new ConnectionFactory();
            opts.ConnectionFactory(connectionFactory);
            return new BrokerConfiguration
            {
                Name = opts.BrokerName,
                Protocol = Constants.Protocol,
                Records = new List<BrokerConfigurationRecord>
                {
                    new BrokerConfigurationRecord
                    {
                        Key = "queueName",
                        Value = opts.QueueName
                    },
                    new BrokerConfigurationRecord
                    {
                        Key = "source",
                        Value = opts.Source
                    },
                    new BrokerConfigurationRecord
                    {
                        Key = "topicName",
                        Value = opts.TopicName
                    },
                    new BrokerConfigurationRecord
                    {
                        Key = "connectionFactory.hostName",
                        Value = connectionFactory.HostName
                    },
                    new BrokerConfigurationRecord
                    {
                        Key = "connectionFactory.port",
                        Value = connectionFactory.Port.ToString()
                    },
                    new BrokerConfigurationRecord
                    {
                        Key = "connectionFactory.username",
                        Value = connectionFactory.UserName
                    },
                    new BrokerConfigurationRecord
                    {
                        Key = "connectionFactory.password",
                        Value = connectionFactory.Password
                    }
                }
            };
        }
    }
}
