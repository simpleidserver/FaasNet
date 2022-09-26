using FaasNet.EventMesh.UI.Stores.App;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.Client
{
    public static class AppReducers
    {
        [ReducerMethod]
        public static AppState Reduce(AppState state, RefreshStatusAction action) => new() { IsLoading = true };

        [ReducerMethod]
        public static AppState ReduceSearchClientsResultAction(AppState state, RefreshStatusResultAction action) => new() { IsLoading = false, LastRefreshTime = action.LastRefreshTime, Nodes = action.Nodes, IsActive = action.IsActive };

        [ReducerMethod]
        public static AppState ReduceSelectActiveNodeAction(AppState state, SelectActiveNodeAction action)
        {
            if (!state.IsActive) return state;
            var node = state.Nodes.FirstOrDefault(n => n.Id == action.Id);
            if (node == null) return state;
            state.SelectedNode = node;
            return state;
        }
    }
}
