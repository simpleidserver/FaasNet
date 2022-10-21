using FaasNet.EventMesh.Client.StateMachines.Client;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.Client
{
    public static class ClientReducers
    {
        [ReducerMethod]
        public static ClientState ReduceSearchClientsAction(ClientState state, SearchClientsAction action) => new(isLoading: true, clients: null);

        [ReducerMethod]
        public static ClientState ReduceGetAllClientsAction(ClientState state, GetAllClientsAction action) => new(isLoading: true, clients: null);

        [ReducerMethod]
        public static ClientState ReduceSearchClientsResultAction(ClientState state, SearchClientsResultAction action) => new(isLoading: false, clients: action.Clients);

        [ReducerMethod]
        public static ClientState ReduceAddClientAction(ClientState state, AddClientAction action)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static ClientState ReduceAddClientFailureAction(ClientState state, AddClientFailureAction action)
        {
            return state with
            {
                IsLoading = false
            };
        }

        [ReducerMethod]
        public static ClientState ReduceAddClientResultAction(ClientState state, AddClientResultAction action)
        {
            var records = state.Clients.Records.ToList();
            records.Insert(0, new ClientViewModel { Id = action.ClientId, Purposes = action.PurposeTypes.Select(p => (ClientPurposeTypes)p).ToList(), Vpn = action.Vpn });
            state.Clients.Records = records;
            return state with
            {
                IsLoading = false,
                Clients = state.Clients
            };
        }

        [ReducerMethod]
        public static ClientState ReduceToggleSelectionClientAction(ClientState state, ToggleSelectionClientAction action)
        {
            var clients = state.Clients.Records.ToList();
            foreach(var client in clients)
            {
                if (client.Id != action.ClientId) client.IsSelected = false;
                else client.IsSelected = !client.IsSelected;
            }

            state.Clients.Records = clients;
            return state with
            {
                Clients = state.Clients
            };
        }
    }
}
