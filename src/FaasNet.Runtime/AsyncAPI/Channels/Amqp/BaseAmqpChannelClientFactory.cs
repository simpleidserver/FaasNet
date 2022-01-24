using RabbitMQ.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Runtime.AsyncAPI.Channels.Amqp
{
    public abstract class BaseAmqpChannelClientFactory : IAmqpChannelClientFactory
    {
        public abstract string SecurityScheme { get; }

        public abstract IConnectionFactory Build(string url, Dictionary<string, string> parameters);

        protected virtual AmqpUrlResult Parse(string url)
        {
            var splitted = url.Split(':');
            int? port = null;
            if (splitted.Count() == 2)
            {
                url = splitted.First();
                port = int.Parse(splitted.Last());
            }

            return new AmqpUrlResult(url, port);
        }
    }
}
