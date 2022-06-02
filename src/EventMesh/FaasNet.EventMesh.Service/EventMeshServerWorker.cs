﻿using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Service
{
    public class EventMeshServerWorker : IHostedService
    {
        private readonly INodeHost _nodeHost;

        public EventMeshServerWorker(INodeHost nodeHost)
        {
            _nodeHost = nodeHost;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _nodeHost.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _nodeHost.Stop();
        }
    }
}
