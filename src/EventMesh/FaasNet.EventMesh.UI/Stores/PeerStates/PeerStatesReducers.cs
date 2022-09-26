using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.PeerStates
{
    public static class PeerStatesReducers
    {
        [ReducerMethod]
        public static PeerStatesState ReduceGetPeerStatesAction(PeerStatesState state, GetPeerStatesAction action) => new(isLoading: true, peerStates: null);

        [ReducerMethod]
        public static PeerStatesState ReduceGetPeerStatesResultAction(PeerStatesState state, GetPeerStatesResultAction action) => new(isLoading: false, peerStates: action.PeerStates);
    }
}
