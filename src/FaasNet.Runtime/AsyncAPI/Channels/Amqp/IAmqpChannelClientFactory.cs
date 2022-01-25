using RabbitMQ.Client;
using System.Collections.Generic;

namespace FaasNet.Runtime.AsyncAPI.Channels.Amqp
{
    public interface IAmqpChannelClientFactory
    {
        string SecurityType { get; }
        IConnectionFactory Build(string url, Dictionary<string, string> parameters);
    }
}
