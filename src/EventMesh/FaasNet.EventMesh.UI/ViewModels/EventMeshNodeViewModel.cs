using FaasNet.Common;
using FaasNet.EventMesh.UI.Data;
using FaasNet.Peer.Client.Transports;
using Microsoft.Extensions.Options;

namespace FaasNet.EventMesh.UI.ViewModels
{
    public class EventMeshNodeViewModel
    {
        private readonly IEventMeshService _eventMeshService;
        private readonly IClientTransport _clientTransport;
        private readonly EventMeshUIOptions _options;
        private System.Timers.Timer _refreshStatusTimer;
        private SemaphoreSlim _lockPeerStates = new SemaphoreSlim(1, 1);

        public EventMeshNodeViewModel(IEventMeshService eventMeshService, IClientTransport clientTransport, IOptions<EventMeshUIOptions> options)
        {
            _eventMeshService = eventMeshService;
            _clientTransport = clientTransport;
            _options = options.Value;
            SelectedNode = new NodeViewModel 
            { 
                Id = PeerId.Build(options.Value.EventMeshUrl, options.Value.EventMeshPort).Serialize(),
                Port = options.Value.EventMeshPort, 
                Url = options.Value.EventMeshUrl 
            };
        }

        public event EventHandler<EventArgs> PropertyChanged;
        public bool IsRunning { get; set; }
        public string Protocol => _clientTransport.Name;
        public DateTime? LastRefreshTime { get; set; }
        public NodeViewModel SelectedNode { get; set; }
        public IEnumerable<NodeViewModel> Nodes { get; set; }
        public IEnumerable<PeerStateViewModel> PeerStates { get; set; }

        public async Task ListenStatus()
        {
            _refreshStatusTimer = new System.Timers.Timer();
            _refreshStatusTimer.Elapsed += RefreshStatus;
            _refreshStatusTimer.Interval = 2000;
            _refreshStatusTimer.AutoReset = false;
            _refreshStatusTimer.Start();
        }

        public void Select(string nodeId)
        {
            SelectedNode = Nodes.Single(n => n.Id == nodeId);
            _lockPeerStates.Wait();
            PeerStates = new List<PeerStateViewModel>();
            _lockPeerStates.Release();
            Notify();
        }

        private async void RefreshStatus(object? sender, System.Timers.ElapsedEventArgs e)
        {
            LastRefreshTime = DateTime.UtcNow;
            var pingResult = await _eventMeshService.Ping(_options.EventMeshUrl, _options.EventMeshPort, CancellationToken.None);
            await RefreshNodes();
            await RefreshStates();
            IsRunning = pingResult;
            Notify();
            _refreshStatusTimer.Start();
        }

        private async Task RefreshNodes()
        {
            if (!IsRunning) return;
            Nodes = (await _eventMeshService.GetAllNodes(_options.EventMeshUrl, _options.EventMeshPort, CancellationToken.None)).Nodes.Select(n => new NodeViewModel
            {
                Id = n.Id,
                Url = n.Url,
                Port = n.Port
            }).OrderBy(n => n.DisplayName);
        }

        private async Task RefreshStates()
        {
            if (!IsRunning) return;
            await _lockPeerStates.WaitAsync();
            PeerStates = (await _eventMeshService.GetAllPeerStates(SelectedNode.Url, SelectedNode.Port, CancellationToken.None)).Select((r) => new PeerStateViewModel
            {
                Name = r.Item2,
                Status = r.Item1.Status,
                CommitIndex = r.Item1.CommitIndex,
                LastApplied = r.Item1.LastApplied,
                SnapshotCommitIndex = r.Item1.SnapshotCommitIndex,
                SnapshotLastApplied = r.Item1.SnapshotLastApplied,
            }).OrderBy(r => r.Name);
            _lockPeerStates.Release();
        }

        private void Notify()
        {
            if (PropertyChanged != null) PropertyChanged(this, EventArgs.Empty);
        }
    }
}
