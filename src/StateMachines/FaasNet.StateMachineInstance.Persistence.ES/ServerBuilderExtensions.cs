using FaasNet.StateMachineInstance.Persistence;
using FaasNet.StateMachineInstance.Persistence.ES;
using FaasNet.StateMachineInstance.Persistence.ES.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseStateMachineInstanceElasticSearchStore(this ServerBuilder serverBuilder, Action<StateMachineInstancePersistenceESOptions> opts = null)
        {
            if (opts == null)
            {
                serverBuilder.Services.Configure<StateMachineInstancePersistenceESOptions>(o => { });
            }
            else
            {
                serverBuilder.Services.Configure(opts);
            }

            serverBuilder.Services.AddTransient<IStateMachineInstanceRepository, ESStateMachineInstanceRepository>();
            return serverBuilder;
        }
    }
}
