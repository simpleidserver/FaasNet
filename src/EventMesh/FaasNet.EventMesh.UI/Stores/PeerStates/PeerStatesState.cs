using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.UI.ViewModels;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.PeerStates
{
    [FeatureState]
    public class PeerStatesState
    {
        public PeerStatesState()
        {

        }

        public IEnumerable<PeerStateViewModel> PeerStates { get; set; }
        public bool IsLoading { get; set; }

        public PeerStatesState(bool isLoading, IEnumerable<PeerStateViewModel> peerStates)
        {
            IsLoading = isLoading;
            PeerStates = peerStates;
        }
    }
}
