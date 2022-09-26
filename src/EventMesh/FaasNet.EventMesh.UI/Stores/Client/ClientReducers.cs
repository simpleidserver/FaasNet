using FaasNet.EventMesh.Client.StateMachines.Client;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.Client
{
    public static class ClientReducers
    {
        [ReducerMethod]
        public static ClientState RdeduceSearchClientsAction(ClientState state, SearchClientsAction action) => new(isLoading: true, clients: null);

        [ReducerMethod]
        public static ClientState ReduceSearchClientsResultAction(ClientState state, SearchClientsResultAction action) => new(isLoading: false, clients: action.Clients);

        [ReducerMethod]
        public static ClientState ReduceAddClientAction(ClientState state, AddClientAction action)
        {
            state.IsLoading = true;
            return state;
        }

        [ReducerMethod]
        public static ClientState ReduceAddClientFailureAction(ClientState state, AddClientFailureAction action)
        {
            state.IsLoading = false;
            return state;
        }

        [ReducerMethod]
        public static ClientState ReduceAddClientResultAction(ClientState state, AddClientResultAction action)
        {
            state.IsLoading = false;
            var records = state.Clients.Records.ToList();
            records.Insert(0, new ClientQueryResult { Id = action.ClientId, Purposes = action.PurposeTypes.Select(p => (ClientPurposeTypes)p).ToList(), Vpn = action.Vpn });
            state.Clients.Records = records;
            return state;
        }
    }
}
