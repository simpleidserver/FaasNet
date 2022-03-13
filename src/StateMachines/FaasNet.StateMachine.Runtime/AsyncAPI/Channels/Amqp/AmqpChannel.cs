using CloudNative.CloudEvents.NewtonsoftJson;
using FaasNet.StateMachine.Runtime.AsyncAPI.Exceptions;
using FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models;
using FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models.Bindings;
using FaasNet.StateMachine.Runtime.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.AsyncAPI.Channels.Amqp
{
    public class AmqpChannel : IChannel
    {
        private readonly IEnumerable<IAmqpChannelClientFactory> _clientFactories;


        public AmqpChannel(IEnumerable<IAmqpChannelClientFactory> clientFactories)
        {
            _clientFactories = clientFactories;
        }

        public string Protocol => "amqp";

        public async Task Invoke(JToken input, Server server, ChannelBindings channelBindings, OperationBindings operationBinding, IEnumerable<SecurityScheme> securitySchemes, Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            var factory = ResolveChannelClientFactory(securitySchemes);
            await PublishMessage(input, server, channelBindings, operationBinding, factory, parameters);
        }

        protected virtual IAmqpChannelClientFactory ResolveChannelClientFactory(IEnumerable<SecurityScheme> securitySchemes)
        {
            var factory = _clientFactories.FirstOrDefault(c => securitySchemes.Any(s => s.Type == c.SecurityType));
            if (factory == null)
            {
                throw new AsyncAPIException(string.Format(Global.UnsupportedSecurityScheme, string.Join(",", securitySchemes.Select(s => s.Type))));
            }

            return factory;
        }

        protected virtual Task PublishMessage(JToken input, Server server, ChannelBindings channelBindings, OperationBindings operationBinding, IAmqpChannelClientFactory amqpChannelClientFactory, Dictionary<string, string> parameters)
        {
            var connectionFactory = amqpChannelClientFactory.Build(server.Url, parameters);
            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var basicProperties = channel.CreateBasicProperties();
                    basicProperties.ContentType = "application/cloudevents+json";
                    var cloudEvent = new CloudNative.CloudEvents.CloudEvent
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = "type",
                        Source = new Uri("http://localhost"),
                        DataContentType = MediaTypeNames.Application.Json,
                        Data = input
                    };
                    var encoded = new JsonEventFormatter().EncodeStructuredModeMessage(cloudEvent, out var contentType);
                    var payload = Encoding.UTF8.GetBytes(cloudEvent.ToString());
                    switch (channelBindings.Amqp.Is)
                    {
                        case v2.Models.Bindings.Amqp.AmqpChannelBindingIs.RoutingKey:
                            var routingKey = operationBinding.Amqp.Cc == null || !operationBinding.Amqp.Cc.Any() ? string.Empty : operationBinding.Amqp.Cc.First();
                            channel.BasicPublish(channelBindings.Amqp.Exchange.Name,
                                routingKey,
                                operationBinding.Amqp.Mandatory,
                                basicProperties,
                                encoded);
                            break;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
