using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.ClientInfo
{
    public static class ClientInfoReducers
    {
        [ReducerMethod]
        public static ClientInfoState ReduceGetClientInfoAction(ClientInfoState state, GetClientInfoAction action) => new(isLoading: true, clientInfo: null);

        [ReducerMethod]
        public static ClientInfoState ReduceSearchClientsResultAction(ClientInfoState state, GetClientInfoResultAction action) => new(isLoading: false, clientInfo: action.ClientInfo);
    }
}
