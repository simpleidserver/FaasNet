using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.UI.Stores.EventDef;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.Client
{
    [FeatureState]
    public record EventDefState
    {
        public EventDefState()
        {

        }

        public GenericSearchQueryResult<EventDefViewModel> EventDefs { get; set; } = new GenericSearchQueryResult<EventDefViewModel>();
        public EventDefViewModel EventDef { get; set; } = new EventDefViewModel();
        public bool IsLoading { get; set; }

        public EventDefState(bool isLoading, GenericSearchQueryResult<EventDefViewModel> eventDefs)
        {
            IsLoading = isLoading;
            EventDefs = eventDefs;
        }
    }
}
