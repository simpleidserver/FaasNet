using FaasNet.EventMesh.Client.StateMachines.Vpn;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.Vpns
{
    public static class VpnsReducers
    {
        [ReducerMethod]
        public static VpnsState RdeduceSearchVpnsAction(VpnsState state, SearchVpnsAction action) => new(isLoading: true, vpns: null);

        [ReducerMethod]
        public static VpnsState ReduceSearchVpnsResultAction(VpnsState state, SearchVpnsResultAction action) => new(isLoading: false, vpns: action.Vpns);

        [ReducerMethod]
        public static VpnsState ReduceAddVpnAction(VpnsState state, AddVpnAction action)
        {
            state.IsLoading = true;
            return state;
        }

        [ReducerMethod]
        public static VpnsState ReduceAddVpnFailureAction(VpnsState state, AddVpnFailureAction action)
        {
            state.IsLoading = false;
            return state;
        }

        [ReducerMethod]
        public static VpnsState ReduceAddVpnResultAction(VpnsState state, AddVpnResultAction action)
        {
            state.IsLoading = false;
            var records = state.Vpns.Records.ToList();
            records.Insert(0, new VpnQueryResult { Description = action.Description, Name = action.Name });
            state.Vpns.Records = records;
            return state;
        }
    }
}
