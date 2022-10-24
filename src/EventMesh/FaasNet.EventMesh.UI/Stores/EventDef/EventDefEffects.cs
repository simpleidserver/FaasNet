using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.UI.Data;
using Fluxor;
using System.ComponentModel.DataAnnotations;

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
            var result = await _eventMeshService.AddEventDefinition(action.Id, action.Vpn, action.JsonSchema, action.Description, action.Topic, action.Url, action.Port, CancellationToken.None);
            if (result.Status != AddEventDefinitionStatus.OK)
            {
                dispatcher.Dispatch(new AddEventDefFailureAction($"An error occured while trying to add the , Error: {Enum.GetName(typeof(AddEventDefinitionStatus), result.Status)}"));
                return;
            }

            dispatcher.Dispatch(new AddEventDefResultAction { JsonSchema = action.JsonSchema, Vpn = action.Vpn, Id = action.Id, Description = action.Description });
        }


        [EffectMethod]
        public async Task Handle(GetEventDefAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.GetEventDefinition(action.Id, action.Vpn, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new GetEventDefResultAction { EventDef = result });
        }

        [EffectMethod]
        public async Task Handle(UpdateEventDefAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.UpdateEventDefinition(action.Id, action.Vpn, action.JsonSchema, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new UpdateEventDefResultAction { Result = result });
        }

        [EffectMethod]
        public async Task Handle(SearchEventDefsAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.SearchEventDefs(action.Filter, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new SearchEventDefsResultAction 
            {
                EventDefinitions = new GenericSearchQueryResult<EventDefViewModel>
                {
                    TotalPages = result.Content.TotalPages,
                    TotalRecords = result.Content.TotalRecords,
                    Records = result.Content.Records.Select(r => new EventDefViewModel(r)).ToList()
                }
            });
        }
    }

    public class AddEventDefAction
    {
        [Required]
        public string Id { get; set; }
        public string Vpn { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Topic { get; set; }
        public string JsonSchema { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }

        public void Reset()
        {
            Id = String.Empty;
            Description = String.Empty;
            JsonSchema = string.Empty;
        }
    }

    public class AddEventDefResultAction
    {
        public string Id { get; set; }
        public string Vpn { get; set; }
        public string JsonSchema { get; set; }
        public string Description { get; set; }
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

    public class UpdateEventDefAction
    {
        public string Id { get; set; }
        public string Vpn { get; set; }
        public string JsonSchema { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class UpdateEventDefResultAction
    {
        public UpdateEventDefinitionResult Result { get; set; }
    }

    public class SearchEventDefsAction
    {
        public FilterQuery Filter { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class SearchEventDefsResultAction
    {
        public GenericSearchQueryResult<EventDefViewModel> EventDefinitions { get; set; }
    }

    public class ToggleSelectionEventDefAction
    {
        public string Id { get; set; }
        public string Vpn { get; set; }
    }
}
