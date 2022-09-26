using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Session;
using FaasNet.EventMesh.UI.Data;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.ClientSessions
{
    public class ClientSessionsEffects
    {
        private readonly IEventMeshService _eventMeshService;

        public ClientSessionsEffects(IEventMeshService eventMeshService)
        {
            _eventMeshService = eventMeshService;
        }

        [EffectMethod]
        public async Task Handle(GetClientSessionsAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.SearchSessions(action.ClientId, action.Vpn, action.Filter, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new GetClientSessionsResultAction(result));
        }
    }

    public class GetClientSessionsAction
    {
        public string ClientId { get; set; }
        public string Vpn { get; set; }
        public FilterQuery Filter { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class GetClientSessionsResultAction
    {
        public GenericSearchQueryResult<SessionQueryResult> Sessions { get; }

        public GetClientSessionsResultAction(GenericSearchQueryResult<SessionQueryResult> sessions)
        {
            Sessions = sessions;
        }
    }
}
