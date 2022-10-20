using FaasNet.EventMesh.UI.Shared.Common;
using FaasNet.EventMesh.UI.Stores.Client;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.EventDef
{
    public class EventDefReducers
    {
        [ReducerMethod]
        public static EventDefState ReduceAddEventDefAction(EventDefState state, AddEventDefAction action)
        {
            return state with
            {
                IsLoading = true
            };
        }

        [ReducerMethod]
        public static EventDefState ReduceAddEventDefResultAction(EventDefState state, AddEventDefResultAction action)
        {
            var records = state.EventDefs.Records.ToList();
            records.Insert(0, new EventDefViewModel { Id = action.Id, JsonSchema = action.JsonSchema, Description = action.Description, Vpn = action.Vpn, IsNew = true });
            state.EventDefs.Records = records;
            return state with
            {
                IsLoading = false,
                EventDefs = state.EventDefs
            };
        }

        [ReducerMethod]
        public static EventDefState ReduceSearchClientsAction(EventDefState state, SearchEventDefsAction action) => new(isLoading: true, eventDefs: null);

        [ReducerMethod]
        public static EventDefState ReduceSearchEventDefsResultAction(EventDefState state, SearchEventDefsResultAction action) => new(isLoading: false, eventDefs: action.EventDefinitions);

        [ReducerMethod]
        public static EventDefState ReduceToggleSelectionEventDefAction(EventDefState state, ToggleSelectionEventDefAction action)
        {
            var evtDefs = state.EventDefs.Records.ToList();
            foreach (var evtDef in evtDefs)
            {
                if (evtDef.Id != action.Id) evtDef.IsSelected = false;
                else evtDef.IsSelected = !evtDef.IsSelected;
            }

            state.EventDefs.Records = evtDefs;
            return state with
            {
                EventDefs = state.EventDefs
            };
        }
    }
}
