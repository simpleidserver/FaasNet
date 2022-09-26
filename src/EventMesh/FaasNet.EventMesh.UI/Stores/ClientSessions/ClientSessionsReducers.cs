using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.ClientSessions
{
    public static class ClientSessionsReducers
    {
        [ReducerMethod]
        public static ClientSessionsState ReduceGetClientSessionsAction(ClientSessionsState state, GetClientSessionsAction action) => new(isLoading: true, sessions: null);

        [ReducerMethod]
        public static ClientSessionsState ReduceGetClientSessionsResultAction(ClientSessionsState state, GetClientSessionsResultAction action) => new(isLoading: false, sessions: action.Sessions);
    }
}
