using FaasNet.Bus.Startup.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saunter;
using Saunter.AsyncApiSchema.v2;
using Saunter.AsyncApiSchema.v2.Bindings;
using Saunter.AsyncApiSchema.v2.Bindings.Amqp;
using System.Collections.Generic;
using System.Reflection;

namespace FaasNet.Bus.Startup
{
    public class Startup
    {
        private const string HOST = "localhost";

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddMassTransit(o =>
            {
                o.AddConsumer<ClientConsumer>();
                o.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(HOST, "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.ReceiveEndpoint("clients", x =>
                    {
                        x.Consumer<ClientConsumer>();
                        x.Bind("submitclient");
                    });
                });
            });
            services.AddMassTransitHostedService(true);
            var security = new List<Dictionary<string, List<string>>>();
            var dic = new Dictionary<string, List<string>>();
            dic.Add("user-password", new List<string>());
            security.Add(dic);
            services.AddAsyncApiSchemaGeneration(options =>
            {
                options.AssemblyMarkerTypes = new[] { typeof(ClientConsumer) };
                options.AsyncApi = new AsyncApiDocument
                {
                    Info = new Info("Bus API", "1.0.0")
                    {

                    },
                    Servers = new Dictionary<string, Server>
                    {
                        {
                            "rabbitmq", new Server($"{HOST}:5672", "amqp")
                            {
                                Security = security
                            }
                        }
                    },
                    Components =
                    {
                        SecuritySchemes = new Dictionary<string, SecurityScheme>
                        {
                            { "user-password", new SecurityScheme(SecuritySchemeType.UserPassword) }
                        },
                        OperationBindings = new Dictionary<string, OperationBindings>
                        {
                            {
                                "addClientOperation", new OperationBindings
                                {
                                    Amqp = new AmqpOperationBinding
                                    {

                                    }
                                }
                            }
                        },
                        ChannelBindings = new Dictionary<string, ChannelBindings>
                        {
                            {
                                "addClientChannel", new ChannelBindings
                                {
                                    Amqp = new AmqpChannelBinding
                                    {
                                        Is = AmqpChannelBindingIs.RoutingKey,
                                        Exchange = new AmqpChannelBindingExchange
                                        {
                                            Name = "submitclient"
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            });
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAsyncApiDocuments();
                endpoints.MapControllers();
            });
        }
    }
}
