using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.UI.Data;
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
            dispatcher.Dispatch(new GetClientInfoResultAction(result));
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
        public GetClientResult ClientInfo { get; }

        public GetClientInfoResultAction(GetClientResult clientInfo)
        {
            ClientInfo = clientInfo;
        }
    }
}
