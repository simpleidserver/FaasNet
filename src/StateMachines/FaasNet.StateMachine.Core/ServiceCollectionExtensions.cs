using FaasNet.StateMachine.Core;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddStateMachine(this IServiceCollection services, Action<StateMachineOptions> callback = null)
        {
            if(callback == null)
            {
                services.Configure<StateMachineOptions>(o => { });
            }
            else
            {
                services.Configure(callback);
            }

            return new ServerBuilder(services);
        }
    }
}
