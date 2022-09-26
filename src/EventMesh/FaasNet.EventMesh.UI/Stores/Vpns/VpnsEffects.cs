using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using FaasNet.EventMesh.UI.Data;
using Fluxor;
using System.ComponentModel.DataAnnotations;

namespace FaasNet.EventMesh.UI.Stores.Vpns
{
    public class VpnsEffects
    {
        private readonly IEventMeshService _eventMeshService;

        public VpnsEffects(IEventMeshService eventMeshService)
        {
            _eventMeshService = eventMeshService;
        }

        [EffectMethod]
        public async Task Handle(SearchVpnsAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.GetAllVpns(action.Filter, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new SearchVpnsResultAction(result));
        }

        [EffectMethod]
        public async Task Handle(AddVpnAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.AddVpn(action.Name, action.Description, action.Url, action.Port, CancellationToken.None);
            if (!result.Success)
            {
                dispatcher.Dispatch(new AddVpnFailureAction($"An error occured while trying to add the VPN, Error: {Enum.GetName(typeof(AddVpnErrorStatus), result.Status.Value)}"));
                return;
            }

            dispatcher.Dispatch(new AddVpnResultAction { Name = action.Name, Description = action.Description });
        }
    }

    public class SearchVpnsAction
    {
        public FilterQuery Filter { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class SearchVpnsResultAction
    {
        public GenericSearchQueryResult<VpnQueryResult> Vpns { get; }

        public SearchVpnsResultAction(GenericSearchQueryResult<VpnQueryResult> vpns)
        {
            Vpns = vpns;
        }
    }

    public class AddVpnAction
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }

        public void Reset()
        {
            Name = String.Empty;
            Description = String.Empty;
        }
    }

    public class AddVpnResultAction
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class AddVpnFailureAction
    {
        public AddVpnFailureAction(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
