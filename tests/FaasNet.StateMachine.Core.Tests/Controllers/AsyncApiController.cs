using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Saunter.AsyncApiSchema.v2;
using Saunter.AsyncApiSchema.v2.Bindings;
using Saunter.AsyncApiSchema.v2.Bindings.Amqp;
using System;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Core.Tests.Controllers
{
    [Route("asyncapi")]
    public class AsyncApiController : Controller
    {
        public class LightMeasuredEvent
        {
            public int Id { get; set; }
            public int Lumens { get; set; }
            public DateTime SentAt { get; set; }
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var security = new List<Dictionary<string, List<string>>>();
            var dic = new Dictionary<string, List<string>>();
            dic.Add("user-password", new List<string>());
            security.Add(dic);
            var payload = NJsonSchema.JsonSchema.FromType<LightMeasuredEvent>();
            var result = new AsyncApiDocument
            {
                Channels = new Dictionary<string, ChannelItem>
                {
                    { "publish/light/measured", new ChannelItem
                    {
                        Bindings = new ChannelBindingsReference("publishLightAmqpChannel"),
                        Servers = new List<string>
                        {
                            "rabbitmq"
                        },
                        Publish = new Operation
                        {
                            OperationId = "PublishLightMeasuredEvent",
                            Description = "description",
                            Summary = "Inform about environmental lighting conditions for a particular streetlight.",
                            Bindings = new OperationBindingsReference("publishLightAmqpOperation"),
                            Message = new MessageReference("msg")
                        }
                    } }
                },
                Servers =
                {
                    { "rabbitmq", new Server("localhost:5672", "amqp")
                    {
                        Security = security
                    } }
                },
                Components =
                {
                    Messages = new Dictionary<string, Message>
                    {
                        { 
                            "msg", new Message
                            {
                                Payload = payload
                            }
                        }
                    },
                    SecuritySchemes = new Dictionary<string, SecurityScheme>
                    {
                        { "user-password", new SecurityScheme(SecuritySchemeType.UserPassword) }
                    },
                    OperationBindings = new Dictionary<string, OperationBindings>
                    {
                        { "publishLightAmqpOperation", new OperationBindings
                        {
                            Amqp = new AmqpOperationBinding
                            {
                                Cc = new string[] { "r1" },
                                Mandatory = false
                            }
                        } }
                    },
                    ChannelBindings = new Dictionary<string, ChannelBindings>
                    {
                        { "publishLightAmqpChannel", new ChannelBindings
                        {
                            Amqp = new AmqpChannelBinding
                            {
                                Is = AmqpChannelBindingIs.RoutingKey,
                                Exchange = new AmqpChannelBindingExchange
                                {
                                    Name = "testExchange",
                                    Type = AmqpChannelBindingExchangeType.Fanout
                                },
                                Queue = new AmqpChannelBindingQueue
                                {
                                    Name = ""
                                }
                            }
                        }}
                    }
                }
            };
            return new OkObjectResult(JsonConvert.SerializeObject(result));
        }
    }
}
