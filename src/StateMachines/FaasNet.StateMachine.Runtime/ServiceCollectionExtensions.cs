using FaasNet.StateMachine.Runtime;
using FaasNet.StateMachine.Runtime.AsyncAPI;
using FaasNet.StateMachine.Runtime.AsyncAPI.Channels;
using FaasNet.StateMachine.Runtime.AsyncAPI.Channels.Amqp;
using FaasNet.StateMachine.Runtime.Factories;
using FaasNet.StateMachine.Runtime.Infrastructure;
using FaasNet.StateMachine.Runtime.OpenAPI;
using FaasNet.StateMachine.Runtime.OpenAPI.Builders;
using FaasNet.StateMachine.Runtime.Processors;
using FaasNet.StateMachine.Runtime.Processors.Functions;
using FaasNet.StateMachine.Runtime.Processors.States;
using v3 = FaasNet.StateMachine.Runtime.OpenAPI.v3;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddRuntime(this IServiceCollection services)
        {
            var serverBuilder = new ServerBuilder(services);
            services.RegisterCore()
                .AddLogging();
            return serverBuilder;
        }

        private static IServiceCollection RegisterCore(this IServiceCollection services)
        {
            services.AddTransient<IRuntimeEngine, RuntimeEngine>();
            services.AddTransient<IActionExecutor, ActionExecutor>();
            services.AddTransient<IStateProcessor, EventStateProcessor>();
            services.AddTransient<IStateProcessor, InjectStateProcessor>();
            services.AddTransient<IStateProcessor, OperationStateProcessor>();
            services.AddTransient<IStateProcessor, SwitchStateProcessor>();
            services.AddTransient<IStateProcessor, ForeachStateProcessor>();
            services.AddTransient<IStateProcessor, CallbackEventStateProcessor>();
            services.AddTransient<IFunctionProcessor, RestApiFunctionProcessor>();
            services.AddTransient<IFunctionProcessor, AsyncApiFunctionProcessor>();
            services.AddTransient<IOpenAPIParser, OpenAPIParser>();
            services.AddTransient<IHttpClientFactory, HttpClientFactory>();
            services.AddTransient<IChannel, AmqpChannel>();
            services.AddTransient<IAmqpChannelClientFactory, AmqpChannelUserPasswordClientFactory>();
            services.AddTransient<IOpenAPIConfigurationParser, v3.OpenAPIConfigurationParser>();
            services.AddTransient<IRequestBodyBuilder, JsonRequestBodyBuilder>();
            services.AddTransient<IAsyncAPIParser, AsyncAPIParser>();
            services.AddSingleton<IDistributedLock, InMemoryDistributedLock>();
            return services;
        }
    }
}
