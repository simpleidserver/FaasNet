using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.UI.Data;
using Fluxor;
using System.ComponentModel.DataAnnotations;

namespace FaasNet.EventMesh.UI.Stores.Client
{
    public class ClientEffects
    {
        private readonly IEventMeshService _eventMeshService;

        public ClientEffects(IEventMeshService eventMeshService)
        {
            _eventMeshService = eventMeshService;
        }

        [EffectMethod]
        public async Task Handle(SearchClientsAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.GetAllClients(action.Filter, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new SearchClientsResultAction(result));
        }

        [EffectMethod]
        public async Task Handle(AddClientAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.AddClient(action.ClientId, action.Vpn, action.PurposeTypes.Select(p => (ClientPurposeTypes)p).ToList(), action.Url, action.Port, CancellationToken.None);
            if (!result.Success)
            {
                dispatcher.Dispatch(new AddClientFailureAction($"An error occured while trying to add the Client, Error: {Enum.GetName(typeof(AddClientErrorStatus), result.Status.Value)}"));
                return;
            }

            dispatcher.Dispatch(new AddClientResultAction { ClientId = action.ClientId, Vpn= action.Vpn, PurposeTypes = action.PurposeTypes, ClientResult = result });
        }
    }

    public class SearchClientsAction
    {
        public FilterQuery Filter { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class SearchClientsResultAction
    {
        public GenericSearchQueryResult<ClientQueryResult> Clients { get; }

        public SearchClientsResultAction(GenericSearchQueryResult<ClientQueryResult> clients)
        {
            Clients = clients;
        }
    }

    public class AddClientAction
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string Vpn { get; set; }
        [Required]
        public int[] PurposeTypes { get; set; } = new int[] { };
        public string Url { get; set; }
        public int Port { get; set; }

        public void Reset()
        {
            ClientId = String.Empty;
            Vpn = String.Empty;
            PurposeTypes = new int[] { };
        }
    }

    public class AddClientResultAction
    {
        public string ClientId { get; set; }
        public string Vpn { get; set; }
        public int[] PurposeTypes { get; set; } = new int[] { };
        public AddClientResult ClientResult { get; set; }
    }

    public class AddClientFailureAction
    {
        public AddClientFailureAction(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
