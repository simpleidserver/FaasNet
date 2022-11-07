using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.ClientInfo
{
    public static class ClientInfoReducers
    {
        [ReducerMethod]
        public static ClientInfoState ReduceGetClientInfoAction(ClientInfoState state, GetClientInfoAction action)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static ClientInfoState ReduceSearchClientsResultAction(ClientInfoState state, GetClientInfoResultAction action)
        {
            return state with
            {
                IsLoading = false,
                Client = action.ClientInfo
            };
        }
    }
}
