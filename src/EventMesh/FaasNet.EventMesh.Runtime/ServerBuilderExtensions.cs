﻿using FaasNet.EventMesh.Runtime;
using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder AddEventMeshServer(this IServiceCollection services, Action<ConsensusNodeOptions> consensusNodeCallback = null, Action<ConsensusPeerOptions> consensusCallback = null, Action<EventMeshNodeOptions> callback = null)
        {
            services.RegisterEventMeshServer(consensusNodeCallback, consensusCallback, callback);
            return new ServerBuilder(services);
        }
    }
}
