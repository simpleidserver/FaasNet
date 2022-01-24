using RabbitMQ.Client;
using System.Collections.Generic;

namespace FaasNet.Runtime.AsyncAPI.Channels.Amqp
{
    public class AmqpChannelUserPasswordClientFactory : BaseAmqpChannelClientFactory
    {
        public override string SecurityScheme => "userPassword";

        public override IConnectionFactory Build(string url, Dictionary<string, string> parameters)
        {
            var urlResult = Parse(url);
            var connectionFactory = new ConnectionFactory
            {
                UserName = parameters["userName"],
                Password = parameters["password"],
                HostName = urlResult.Url
            };
            if (urlResult.Port != null)
            {
                connectionFactory.Port = urlResult.Port.Value;
            }

            return connectionFactory;
        }
    }
}
