﻿using FaasNet.EventMesh.Client.StateMachines.Client;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.Client
{
    public static class ClientReducers
    {
        [ReducerMethod]
        public static ClientState RdeduceSearchClientsAction(ClientState state, SearchClientsAction action) => new(isLoading: true, clients: null);

        [ReducerMethod]
        public static ClientState ReduceGetAllClientsAction(ClientState state, GetAllClientsAction action) => new(isLoading: true, clients: null);

        [ReducerMethod]
        public static ClientState ReduceSearchClientsResultAction(ClientState state, SearchClientsResultAction action) => new(isLoading: false, clients: action.Clients);

        [ReducerMethod]
        public static ClientState ReduceAddClientAction(ClientState state, AddClientAction action)
        {
            state.IsLoading = true;
            return state;
        }

        [ReducerMethod]
        public static ClientState ReduceUpdateClientAction(ClientState state, BulkUpdateClientAction action)
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

        [ReducerMethod]
        public static ClientState ReduceUpdateClientResultAction(ClientState state, BulkUpdateClientResultAction action)
        {
            state.IsLoading = false;
            return state;
        }

        [ReducerMethod]
        public static ClientState ReduceRemoveClientResultAction(ClientState state, RemoveClientResultAction action)
        {
            state.IsLoading = false;
            if (!action.IsRemoved) return state;
            var records = state.Clients.Records.ToList();
            records.Remove(state.Clients.Records.First(r => r.Id == action.ClientId && r.Vpn == action.Vpn));
            state.Clients.Records = records;
            return state;
        }
    }
}
