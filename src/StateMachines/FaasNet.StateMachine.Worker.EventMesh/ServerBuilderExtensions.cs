using FaasNet.Common;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.StateMachine.Worker.EventMesh
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
