using FaasNet.StateMachine.Worker;
using FaasNet.StateMachine.Worker.EventMesh;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseEventMesh(this ServerBuilder serverBuilder, Action<EventMeshOptions> callbackOptions = null)
        {
            if(callbackOptions == null)
            {
                serverBuilder.Services.Configure<EventMeshOptions>((o) => { });
            }
            else
            {
                serverBuilder.Services.Configure(callbackOptions);
            }

            serverBuilder.Services.AddTransient<IMessageListener, EventMeshMessageListener>();
            return serverBuilder;
        }
    }
}
