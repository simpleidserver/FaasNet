using FaasNet.EventMesh.UI.Data;
using FaasNet.EventMesh.UI.ViewModels;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.PeerStates
{
    public class PeerStatesEffects
    {
        private readonly IEventMeshService _eventMeshService;

        public PeerStatesEffects(IEventMeshService eventMeshService)
        {
            _eventMeshService = eventMeshService;
        }

        [EffectMethod]
        public async Task Handle(GetPeerStatesAction action, IDispatcher dispatcher)
        {
            var result = (await _eventMeshService.GetAllPeerStates(action.Url, action.Port, CancellationToken.None)).Select((r) => new PeerStateViewModel
            {
                Name = r.Item2,
                Status = r.Item1.Status,
                CommitIndex = r.Item1.CommitIndex,
                LastApplied = r.Item1.LastApplied,
                SnapshotCommitIndex = r.Item1.SnapshotCommitIndex,
                SnapshotLastApplied = r.Item1.SnapshotLastApplied,
            }).OrderBy(r => r.Name);
            dispatcher.Dispatch(new GetPeerStatesResultAction(result));
        }
    }

    public class GetPeerStatesAction
    {
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class GetPeerStatesResultAction
    {
        public IEnumerable<PeerStateViewModel> PeerStates { get; }

        public GetPeerStatesResultAction(IEnumerable<PeerStateViewModel> peerStates)
        {
            PeerStates = peerStates;
        }
    }
}
