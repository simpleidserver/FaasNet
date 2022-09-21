using FaasNet.Common;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
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
            _refreshStatusTimer = new System.Timers.Timer();
            _refreshStatusTimer.Elapsed += RefreshStatus;
            _refreshStatusTimer.Interval = 2000;
            _refreshStatusTimer.AutoReset = false;
        }

        public event EventHandler<EventArgs> StatusChanged;
        public event EventHandler<EventArgs> NodesRefreshed;
        public event EventHandler<EventArgs> SelectedNodeChanged;
        public bool IsRunning { get; set; }
        public string Protocol => _clientTransport.Name;
        public DateTime? LastRefreshTime { get; set; }
        public NodeViewModel SelectedNode { get; set; }
        public IEnumerable<NodeViewModel> Nodes { get; set; } = new List<NodeViewModel>();
        public IEnumerable<PeerStateViewModel> PeerStates { get; set; } = new List<PeerStateViewModel>();
        public GenericSearchQueryResult<VpnQueryResult> Vpns { get; set; } = new GenericSearchQueryResult<VpnQueryResult>();
        public GenericSearchQueryResult<ClientQueryResult> Clients { get; set; } = new GenericSearchQueryResult<ClientQueryResult>();

        public void ListenStatus()
        {
            _refreshStatusTimer.Start();
        }

        public void ResetStates()
        {
            PeerStates = new List<PeerStateViewModel>();
        }

        public async Task RefreshStates()
        {
            if (!IsRunning) return;
            PeerStates = (await _eventMeshService.GetAllPeerStates(SelectedNode.Url, SelectedNode.Port, CancellationToken.None)).Select((r) => new PeerStateViewModel
            {
                Name = r.Item2,
                Status = r.Item1.Status,
                CommitIndex = r.Item1.CommitIndex,
                LastApplied = r.Item1.LastApplied,
                SnapshotCommitIndex = r.Item1.SnapshotCommitIndex,
                SnapshotLastApplied = r.Item1.SnapshotLastApplied,
            }).OrderBy(r => r.Name);
        }

        public void ResetClients()
        {
            Clients = new GenericSearchQueryResult<ClientQueryResult>();
        }

        public async Task RefreshClients(FilterQuery filter)
        {
            if (!IsRunning) return;
            Clients = await _eventMeshService.GetAllClients(filter, SelectedNode.Url, SelectedNode.Port, CancellationToken.None);
        }

        public void ResetVpns()
        {
            Vpns = new GenericSearchQueryResult<VpnQueryResult>();
        }

        public async Task RefreshVpns(FilterQuery filter)
        {
            if (!IsRunning) return;
            Vpns = await _eventMeshService.GetAllVpns(filter, SelectedNode.Url, SelectedNode.Port, CancellationToken.None);
        }

        public void Select(string nodeId)
        {
            SelectedNode = Nodes.Single(n => n.Id == nodeId);
            if (SelectedNodeChanged != null) SelectedNodeChanged(this, EventArgs.Empty);
        }

        public async Task<AddVpnResult> AddVpn(AddVpnViewModel addVpn)
        {
            var result = await _eventMeshService.AddVpn(addVpn.Name, addVpn.Description, SelectedNode.Url, SelectedNode.Port, CancellationToken.None);
            return result;
        }

        public async Task<AddClientResult> AddClient(AddClientViewModel addClient)
        {
            var result = await _eventMeshService.AddClient(addClient.ClientId, addClient.Vpn, addClient.PurposeTypes.Select(p =>(ClientPurposeTypes)p).ToList(), SelectedNode.Url, SelectedNode.Port, CancellationToken.None);
            return result;
        }

        private async void RefreshStatus(object? sender, System.Timers.ElapsedEventArgs e)
        {
            LastRefreshTime = DateTime.UtcNow;
            var pingResult = await _eventMeshService.Ping(_options.EventMeshUrl, _options.EventMeshPort, CancellationToken.None);
            await RefreshNodes();
            bool isChanged = pingResult != IsRunning && StatusChanged != null;
            IsRunning = pingResult;
            if (isChanged) StatusChanged(this, EventArgs.Empty);
            _refreshStatusTimer.Start();

            async Task RefreshNodes()
            {
                if (!IsRunning) return;
                Nodes = (await _eventMeshService.GetAllNodes(_options.EventMeshUrl, _options.EventMeshPort, CancellationToken.None)).Nodes.Select(n => new NodeViewModel
                {
                    Id = n.Id,
                    Url = n.Url,
                    Port = n.Port
                }).OrderBy(n => n.DisplayName);
                if (NodesRefreshed != null) NodesRefreshed(this, EventArgs.Empty);
            }
        }
    }
}
