using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Session;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.ClientSessions
{
    [FeatureState]
    public class ClientSessionsState
    {
        public ClientSessionsState()
        {

        }

        public GenericSearchQueryResult<SessionQueryResult> Sessions { get; set; }
        public bool IsLoading { get; set; }

        public ClientSessionsState(bool isLoading, GenericSearchQueryResult<SessionQueryResult> sessions)
        {
            IsLoading = isLoading;
            Sessions = sessions;
        }
    }
}
