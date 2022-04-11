using FaasNet.Common;
using FaasNet.EventStore;
using FaasNet.StateMachine.Exporter;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddStateMachineExporter(this IServiceCollection services, Action<StateMachineExporterOptions> options = null)
        {
            if (options == null)
            {
                services.Configure<StateMachineExporterOptions>(o => { });
            }
            else
            {
                services.Configure(options);
            }

            services.AddEventStore();
            services.AddTransient<IQueryProjection, StateMachineInstanceQueryProjection>();
            services.AddTransient<IProjectionHostedJob, ProjectionHostedJob>();
            return new ServerBuilder(services);
        }
    }
}
