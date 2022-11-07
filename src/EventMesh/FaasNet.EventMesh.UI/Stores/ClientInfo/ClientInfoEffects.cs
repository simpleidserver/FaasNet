using FaasNet.EventMesh.UI.Data;
using FaasNet.EventMesh.UI.Stores.Client;
using Fluxor;

namespace FaasNet.EventMesh.UI.Stores.ClientInfo
{
    public class ClientInfoEffects
    {
        private readonly IEventMeshService _eventMeshService;

        public ClientInfoEffects(IEventMeshService eventMeshService)
        {
            _eventMeshService = eventMeshService;
        }

        [EffectMethod]
        public async Task Handle(GetClientInfoAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.GetClient(action.ClientId, action.Vpn, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new GetClientInfoResultAction(new ClientViewModel(result.Content)));
        }
    }

    public class GetClientInfoAction
    {
        public string ClientId { get; set; }
        public string Vpn { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class GetClientInfoResultAction
    {
        public ClientViewModel ClientInfo { get; }

        public GetClientInfoResultAction(ClientViewModel clientInfo)
        {
            ClientInfo = clientInfo;
        }
    }
}
