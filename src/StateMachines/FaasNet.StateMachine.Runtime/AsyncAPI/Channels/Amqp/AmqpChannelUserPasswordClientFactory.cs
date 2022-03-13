using RabbitMQ.Client;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Runtime.AsyncAPI.Channels.Amqp
{
    public class AmqpChannelUserPasswordClientFactory : BaseAmqpChannelClientFactory
    {
        public static string SECURITY_TYPE = "userPassword";

        public override string SecurityType => "userPassword";

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
