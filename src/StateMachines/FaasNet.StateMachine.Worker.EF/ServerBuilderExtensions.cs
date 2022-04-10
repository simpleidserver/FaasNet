using FaasNet.StateMachine.Worker.EF;
using FaasNet.StateMachine.Worker.EF.Persistence;
using FaasNet.StateMachine.Worker.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseEF(this ServerBuilder serverBuilder, Action<DbContextOptionsBuilder> optionsBuilder = null)
        {
            serverBuilder.Services.AddTransient<ICloudEventSubscriptionRepository, EFCloudEventSubscriptionRepository>();
            serverBuilder.Services.AddDbContext<WorkerDBContext>(optionsBuilder);
            return serverBuilder;
        }
    }
}
