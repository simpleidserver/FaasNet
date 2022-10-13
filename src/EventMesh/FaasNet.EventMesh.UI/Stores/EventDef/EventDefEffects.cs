using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.UI.Data;
using FaasNet.EventMesh.UI.Stores.Client;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.EventDef
{
    public class EventDefEffects
    {
        private readonly IEventMeshService _eventMeshService;

        public EventDefEffects(IEventMeshService eventMeshService)
        {
            _eventMeshService = eventMeshService;
        }


        [EffectMethod]
        public async Task Handle(AddEventDefAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.AddEventDefinition(action.Vpn, action.JsonSchema, action.Source, action.Target, action.Url, action.Port, CancellationToken.None);
            if (result.Status != AddEventDefinitionStatus.OK)
            {
                dispatcher.Dispatch(new AddEventDefFailureAction($"An error occured while trying to add the , Error: {Enum.GetName(typeof(AddEventDefinitionStatus), result.Status)}"));
                return;
            }

            dispatcher.Dispatch(new AddEventDefResultAction { EventDef = result });
        }


        [EffectMethod]
        public async Task Handle(GetEventDefAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.GetEventDefinition(action.Id, action.Vpn, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new GetEventDefResultAction { EventDef = result });
        }
    }

    public class AddEventDefAction
    {
        public string Vpn { get; set; }
        public string JsonSchema { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class AddEventDefResultAction
    {
        public AddEventDefinitionResult EventDef { get; set; }
    }

    public class AddEventDefFailureAction
    {
        public AddEventDefFailureAction(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }

    public class GetEventDefAction
    {
        public string Id { get; set; }
        public string Vpn { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class GetEventDefResultAction
    {
        public GetEventDefinitionResult EventDef { get; set; }
    }
}
