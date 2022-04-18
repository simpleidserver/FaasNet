using FaasNet.StateMachine.Runtime;
using FaasNet.StateMachine.Runtime.AsyncAPI;
using FaasNet.StateMachine.Runtime.AsyncAPI.Channels;
using FaasNet.StateMachine.Runtime.AsyncAPI.Channels.Amqp;
using FaasNet.StateMachine.Runtime.Factories;
using FaasNet.StateMachine.Runtime.OpenAPI;
using FaasNet.StateMachine.Runtime.Processors;
using FaasNet.StateMachine.Runtime.Processors.Functions;
using FaasNet.StateMachine.Runtime.Processors.States;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStateMachineRuntimeCore(this IServiceCollection services)
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
            services.AddTransient<IRequestBodyBuilder, JsonRequestBodyBuilder>();
            services.AddTransient<IAsyncAPIParser, AsyncAPIParser>();
            return services;
        }
    }
}
