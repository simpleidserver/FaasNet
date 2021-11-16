using FaasNet.Runtime;
using FaasNet.Runtime.EF;
using FaasNet.Runtime.EF.Persistence;
using FaasNet.Runtime.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddRuntimeEF(this ServerBuilder builder, Action<DbContextOptionsBuilder> optionsBuilder = null)
        {
            var services = builder.Services;
            services.AddDbContext<RuntimeDBContext>();
            services.AddTransient<ICloudEventSubscriptionRepository, CloudEventSubscriptionRepository>();
            services.AddTransient<IWorkflowDefinitionRepository, WorkflowDefinitionRepository>();
            services.AddTransient<IWorkflowInstanceRepository, WorkflowInstanceRepository>();
            return builder;
        }
    }
}
