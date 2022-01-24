using FaasNet.Runtime.AsyncAPI.Channels;
using FaasNet.Runtime.AsyncAPI.Exceptions;
using FaasNet.Runtime.AsyncAPI.v2.Converters;
using FaasNet.Runtime.AsyncAPI.v2.Models;
using FaasNet.Runtime.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.AsyncAPI
{
    public class AsyncAPIParser : IAsyncAPIParser
    {
        private readonly Factories.IHttpClientFactory _httpClientFactory;
        private readonly IEnumerable<IChannel> _channels;

        public AsyncAPIParser(
            Factories.IHttpClientFactory httpClientFactory,
            IEnumerable<IChannel> channels)
        {
            _httpClientFactory = httpClientFactory;
            _channels = channels;
        }

        public bool TryParseUrl(string url, out AsyncApiResult operationResult)
        {
            var splitted = url.Split('#');
            if (splitted.Count() != 2)
            {
                operationResult = null;
                return false;
            }

            operationResult = new AsyncApiResult(splitted.First(), splitted.Last());
            return true;
        }

        public async Task Invoke(string url, string operationId, JToken input, Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.Build();
            var httpResult = await httpClient.GetAsync(url, cancellationToken);
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var settings = new JsonSerializerSettings
            {
                ReferenceResolverProvider = () => new AsyncApiReferenceResolver()
            };
            var doc = JsonConvert.DeserializeObject<AsyncApiDocument>(json, settings);
            var channel = doc.Channels.FirstOrDefault(s => s.Value != null && s.Value.Publish != null && s.Value.Publish.OperationId == operationId);
            if(channel.Equals(default(KeyValuePair<string, ChannelItem>)) || channel.Value == null)
            {
                throw new AsyncAPIException(string.Format(Global.UnknownAsyncAPIOperation, operationId));
            }

            var message = (channel.Value.Publish.Message as MessageReference).Reference;
            var validationResult = message.Payload.Validate(input);
            if (validationResult.Any())
            {
                throw new AsyncAPIException(string.Join(',', validationResult.Select(r => $"{r.Kind}:{r.Path}")));
            }

            var server = ResolveServer(channel, doc);
            var ch = _channels.First(c => c.Protocol == server.Protocol);
            // TODO : Add BINDING into Channel !!!
            // channel.Value.
        }

        private Server ResolveServer(KeyValuePair<string, ChannelItem> kvp, AsyncApiDocument doc)
        {
            var serverName = kvp.Value.Servers?.FirstOrDefault(s =>
            {
                var server = doc.Servers.FirstOrDefault(se => se.Key == s);
                if (server.Equals(default(KeyValuePair<string, Server>)) || server.Value == null)
                {
                    return false;
                }

                return _channels.Any(c => c.Protocol == server.Value.Protocol);
            });

            var server = doc.Servers.FirstOrDefault(s => ((!string.IsNullOrWhiteSpace(serverName) && s.Key == serverName) || string.IsNullOrWhiteSpace(serverName)) && _channels.Any(c => c.Protocol == s.Value.Protocol));
            if (server.Equals(default(KeyValuePair<string, Server>)) || server.Value == null)
            {
                throw new AsyncAPIException(Global.UnsupportedProtocol);
            }

            return server.Value;
        }
    }
}
