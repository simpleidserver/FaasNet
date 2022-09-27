using FaasNet.EventMesh.UI.Stores.App;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.Client
{
    public static class AppReducers
    {
        [ReducerMethod]
        public static AppState Reduce(AppState state, RefreshStatusAction action)
        {
            state.IsLoading = true;
            return state;
        }

        [ReducerMethod]
        public static AppState ReduceRefreshStatusResultAction(AppState state, RefreshStatusResultAction action)
        {
            state.IsLoading = false;
            state.LastRefreshTime = DateTime.UtcNow;
            state.Nodes = action.Nodes;
            state.IsActive = action.IsActive;
            return state;
        }

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
