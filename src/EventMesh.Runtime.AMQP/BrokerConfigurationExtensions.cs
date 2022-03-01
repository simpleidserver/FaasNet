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
                    var hostName = conf.GetValue("connectionFactory.hostName");
                    if (!string.IsNullOrWhiteSpace(hostName))
                    {
                        opt.HostName = hostName;
                    }

                    var port = int.Parse(conf.GetValue("connectionFactory.port"));
                    if (port == -1)
                    {
                        port = 5672;
                    }

                    opt.Port = port;
                    var userName = conf.GetValue("connectionFactory.username");
                    if (!string.IsNullOrWhiteSpace(userName))
                    {
                        opt.UserName = userName;
                    }

                    var password = conf.GetValue("connectionFactory.password");
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        opt.Password = password;
                    }
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
