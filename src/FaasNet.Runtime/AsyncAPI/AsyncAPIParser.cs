using FaasNet.Runtime.AsyncAPI.Channels;
using FaasNet.Runtime.AsyncAPI.Exceptions;
using FaasNet.Runtime.AsyncAPI.v2.Models;
using FaasNet.Runtime.AsyncAPI.v2.Models.Bindings;
using FaasNet.Runtime.JSchemas;
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
            var doc = await GetConfiguration(url, cancellationToken);
            var channel = GetChannel(doc, operationId);
            var channelBindings = GetChannelBindings(channel);
            var operationBindings = GetOperationBindings(channel.Publish);
            var server = GetServer(channel, doc);
            var securitySchemes = GetSecuritySchemes(doc, server);
            ValidateMessage(input, channel);
            var ch = _channels.Single(c => c.Protocol == server.Protocol);
            await ch.Invoke(input, server, channelBindings, operationBindings, securitySchemes, parameters, cancellationToken);
        }

        protected virtual async Task<AsyncApiDocument> GetConfiguration(string url, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.Build();
            var httpResult = await httpClient.GetAsync(url, cancellationToken);
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var settings = new JsonSerializerSettings
            {
                ReferenceResolverProvider = () => new FaasNetReferenceResolver()
            };
            var doc = JsonConvert.DeserializeObject<AsyncApiDocument>(json, settings);
            return doc;
        }

        protected virtual ChannelItem GetChannel(AsyncApiDocument doc, string operationId)
        {
            var channel = doc.Channels.FirstOrDefault(s => s.Value != null && s.Value.Publish != null && s.Value.Publish.OperationId == operationId);
            if (channel.Equals(default(KeyValuePair<string, ChannelItem>)) || channel.Value == null)
            {
                throw new AsyncAPIException(string.Format(Global.UnknownAsyncAPIOperation, operationId));
            }

            return channel.Value;
        }

        protected virtual ChannelBindings GetChannelBindings(ChannelItem channel)
        {
            var binding = channel.Bindings;
            if (binding.Reference != null)
            {
                binding = binding.Reference;
            }

            return binding;
        }

        protected virtual OperationBindings GetOperationBindings(Operation operation)
        {
            var binding = operation.Bindings;
            if (binding.Reference != null)
            {
                binding = binding.Reference;
            }

            return binding;
        }

        protected virtual Server GetServer(ChannelItem channel, AsyncApiDocument doc)
        {
            var serverName = channel.Servers?.FirstOrDefault(s =>
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

        protected virtual IEnumerable<SecurityScheme> GetSecuritySchemes(AsyncApiDocument doc, Server server)
        {
            return doc.Components.SecuritySchemes.Where(kvp => server.Security.Any(rec => rec.ContainsKey(kvp.Key)))
                .Select(s => s.Value);
        }


        protected virtual void ValidateMessage(JToken input, ChannelItem item)
        {
            var message = item.Publish.Message.Payload != null ? item.Publish.Message : item.Publish.Message.Reference;
            if (message.Payload == null && message.Reference != null)
            {
                message = message.Reference;
            }

            var validationResult = message.Payload.Validate(input);
            if (validationResult.Any())
            {
                throw new AsyncAPIException(string.Join(',', validationResult.Select(r => $"{r.Kind}:{r.Path}")));
            }
        }
    }
}
