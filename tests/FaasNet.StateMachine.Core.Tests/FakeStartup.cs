using FaasNet.StateMachine.Core.Tests.Bus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Saunter;
using Saunter.AsyncApiSchema.v2;
using Saunter.AsyncApiSchema.v2.Bindings;
using Saunter.AsyncApiSchema.v2.Bindings.Amqp;
using System.Collections.Generic;

namespace FaasNet.StateMachine.Core.Tests
{
    public class FakeStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.AddControllers();
            var security = new List<Dictionary<string, List<string>>>();
            var dic = new Dictionary<string, List<string>>();
            dic.Add("user-password", new List<string>());
            security.Add(dic);
            services.AddAsyncApiSchemaGeneration(options =>
            {
                // Specify example type(s) from assemblies to scan.
                options.AssemblyMarkerTypes = new[] { typeof(StreetlightMessageBus) };
                options.AsyncApi = new AsyncApiDocument
                {
                    Servers =
                    {
                        { "rabbitmq", new Server("localhost:5672", "amqp")
                        {
                            Security = security
                        }}
                    },
                    Components =
                    {
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
                                    Cc = new string[] { "r1" }
                                }
                            } }
                        },
                        ChannelBindings = new Dictionary<string, ChannelBindings>
                        {
                            { "publishLightAmqpChannel", new ChannelBindings
                            {
                                Amqp = new AmqpChannelBinding
                                {
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
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway V1");
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAsyncApiDocuments();
                endpoints.MapControllers();
            });
        }
    }
}
