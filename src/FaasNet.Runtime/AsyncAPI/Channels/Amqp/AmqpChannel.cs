using FaasNet.Runtime.AsyncAPI.Exceptions;
using FaasNet.Runtime.AsyncAPI.v2.Models;
using FaasNet.Runtime.Resources;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaasNet.Runtime.AsyncAPI.Channels.Amqp
{
    public class AmqpChannel : IChannel
    {
        private readonly IEnumerable<IAmqpChannelClientFactory> _clientFactories;


        public AmqpChannel(IEnumerable<IAmqpChannelClientFactory> clientFactories)
        {
            _clientFactories = clientFactories;
        }

        public string Protocol => "amqp";

        public Task Invoke(JToken input, Server server, SecurityScheme securityScheme, Dictionary<string, string> parameters)
        {
            var factory = _clientFactories.FirstOrDefault(c => c.SecurityScheme == securityScheme.Scheme);
            if (factory == null)
            {
                throw new AsyncAPIException(string.Format(Global.UnsupportedSecurityScheme, securityScheme.Scheme));
            }

            var connectionFactory = factory.Build(server.Url, parameters);
            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    // channel.ExchangeDeclare();
                    channel.BasicPublish("exchange", "", true, null, null);
                }
            }

            return Task.CompletedTask;
        }
    }
}
