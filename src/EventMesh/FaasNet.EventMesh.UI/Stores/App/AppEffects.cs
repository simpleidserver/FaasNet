﻿using FaasNet.EventMesh.UI.Data;
using FaasNet.EventMesh.UI.Stores.App;
using Fluxor;
using Microsoft.Extensions.Options;

namespace FaasNet.EventMesh.UI.Stores.Client
{
    public class AppEffects
    {
        private readonly IEventMeshService _eventMeshService;
        private readonly EventMeshUIOptions _options;

        public AppEffects(IEventMeshService eventMeshService, IOptions<EventMeshUIOptions> options)
        {
            _eventMeshService = eventMeshService;
            _options = options.Value;
        }

        [EffectMethod]
        public async Task Handle(RefreshStatusAction action, IDispatcher dispatcher)
        {
            var lastRefreshTime = DateTime.UtcNow;
            var pingResult = await _eventMeshService.Ping(_options.EventMeshUrl, _options.EventMeshPort, CancellationToken.None);
            if (!pingResult)
            {
                dispatcher.Dispatch(new RefreshStatusResultAction(lastRefreshTime));
                return;
            }

            var result = (await _eventMeshService.GetAllNodes(_options.EventMeshUrl, _options.EventMeshPort, CancellationToken.None)).Nodes.Select(n => new EventMeshNode
            {
                Id = n.Id,
                Port = n.Port,
                Url = n.Url
            });
            dispatcher.Dispatch(new RefreshStatusResultAction(result, lastRefreshTime));
        }
    }

    public class RefreshStatusAction
    {

    }

    public class RefreshStatusResultAction
    {
        public bool IsActive { get; }
        public IEnumerable<EventMeshNode> Nodes { get; } = new List<EventMeshNode>();
        public DateTime LastRefreshTime { get; }

        public RefreshStatusResultAction(DateTime lastRefreshTime)
        {
            IsActive = false;
            LastRefreshTime = lastRefreshTime;
        }

        public RefreshStatusResultAction(IEnumerable<EventMeshNode> nodes, DateTime lastRefreshTime)
        {
            IsActive = true;
            Nodes = nodes;
            LastRefreshTime = lastRefreshTime;
        }
    }

    public class SelectActiveNodeAction
    {
        public string Id { get; set; }
    }
}
