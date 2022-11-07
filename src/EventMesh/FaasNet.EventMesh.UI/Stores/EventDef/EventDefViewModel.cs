using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;

namespace FaasNet.EventMesh.UI.Stores.EventDef
{
    public class EventDefViewModel : EventDefinitionQueryResult
    {
        public EventDefViewModel() { }

        public EventDefViewModel(EventDefinitionQueryResult result)
        {
            Id = result.Id;
            Vpn = result.Vpn;
            JsonSchema = result.JsonSchema;
            CreateDateTime = result.CreateDateTime;
            UpdateDateTime = result.UpdateDateTime;
            Links = result.Links;
            Topic = result.Topic;
            Description = result.Description;
        }

        public bool IsSelected { get; set; }
        public bool IsNew { get; set; } = false;
        public GenericSearchQueryResult<EventDefinitionLinkResult> LinksResult
        {
            get
            {
                return new GenericSearchQueryResult<EventDefinitionLinkResult>
                {
                    Records = Links
                };
            }
        }
    }
}
