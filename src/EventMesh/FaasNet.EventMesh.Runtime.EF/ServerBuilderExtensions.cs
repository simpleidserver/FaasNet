using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.EventMesh.Runtime
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseEF(this ServerBuilder serverBuilder, Action<DbContextOptionsBuilder> options = null)
        {
            serverBuilder.Services.AddRuntimeEF(options);
            return serverBuilder;
        }

        public static ServerBuilder UseEFMessageBroker(this ServerBuilder serverBuilder, Action<DbContextOptionsBuilder> options = null)
        {
            serverBuilder.Services.AddMessageBrokerEF(options);
            return serverBuilder;
        }
    }
}
