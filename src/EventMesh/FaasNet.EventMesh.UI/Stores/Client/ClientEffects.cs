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
            var result = await _eventMeshService.AddClient(action.ClientId, action.Vpn, action.PurposeTypes.Select(p => (ClientPurposeTypes)p).ToList(), action.Url, action.Port, CancellationToken.None, action.CoordinateX, action.CoordinateY);
            if (!result.Success)
            {
                dispatcher.Dispatch(new AddClientFailureAction($"An error occured while trying to add the Client, Error: {Enum.GetName(typeof(AddClientErrorStatus), result.Status.Value)}"));
                return;
            }

            dispatcher.Dispatch(new AddClientResultAction { ClientId = action.ClientId, Vpn= action.Vpn, PurposeTypes = action.PurposeTypes, ClientResult = result });
        }

        [EffectMethod]
        public async Task Handle(BulkUpdateClientAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.BulkUpdateClient(action.Vpn, action.Clients.Select(c => new UpdateClientRequest
            {
                CoordinateX = c.CoordinateX,
                CoordinateY = c.CoordinateY,
                Id = c.ClientId,
                Targets = c.Targets.Select(t => new UpdateClientTarget
                {
                    Target = t.Target,
                    EventId = t.EventId
                }).ToList()
            }).ToList(), action.Url, action.Port, CancellationToken.None);
            if (!result.Success)
            {
                dispatcher.Dispatch(new AddClientFailureAction($"An error occured while trying to update the Client, Error: {Enum.GetName(typeof(UpdateClientErrorStatus), result.Status.Value)}"));
                return;
            }

            dispatcher.Dispatch(new BulkUpdateClientResultAction { Clients = action.Clients, Vpn = action.Vpn, ClientResult = result });
        }

        [EffectMethod]
        public async Task Handle(GetAllClientsAction action, IDispatcher dispatcher)
        {
            var filterQuery = new FilterQuery
            {
                NbRecords = 100,
                Page = 0,
                HasComparison = true,
                Comparison = new ComparisonExpression
                {
                    Key = "Vpn",
                    Value = action.Vpn,
                    Operator = ComparisonOperators.EQUAL
                },
                SortBy = "CreateDateTime",
                SortOrder = SortOrders.DESC
            };
            var result = await _eventMeshService.GetAllClients(filterQuery, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new SearchClientsResultAction(result));
        }

        [EffectMethod]
        public async Task Handle(CheckClientPartitionIsSyncedAction action, IDispatcher dispatcher)
        {
            var result = await _eventMeshService.GetPartition("CLIENT", action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new CheckClientPartitionIsSyncedResultAction { IsSynced = result.State.CommitIndex == result.State.LastApplied });
        }

        [EffectMethod]
        public async Task Handle(RemoveClientAction action, IDispatcher dispatcher)
        {
            var cl = await _eventMeshService.RemoveClient(action.Vpn, action.ClientId, action.Url, action.Port, CancellationToken.None);
            dispatcher.Dispatch(new RemoveClientResultAction { ClientId = action.ClientId, Vpn = action.Vpn, IsRemoved = cl.Status == RemoveClientStatus.OK, Status = cl.Status });
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
        public string ClientId { get; set; } = string.Empty;
        [Required]
        public string Vpn { get; set; } = string.Empty;
        [Required]
        public int[] PurposeTypes { get; set; } = new int[] { };
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Port { get; set; }

        public void Reset()
        {
            ClientId = string.Empty;
            Vpn = string.Empty;
            PurposeTypes = new int[] { };
        }
    }

    public class BulkUpdateClientAction
    {
        public string Vpn { get; set; } = string.Empty;
        public ICollection<UpdateClientSubAction> Clients { get; set; } = new List<UpdateClientSubAction>();
        public string Url { get; set; } = string.Empty;
        public int Port { get; set; }
    }

    public class UpdateClientSubAction
    {
        public string ClientId { get; set; } = string.Empty;
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public ICollection<UpdateClientSubTarget> Targets { get; set; }
    }

    public class UpdateClientSubTarget
    {
        public string EventId { get; set; }
        public string Target { get; set; }
    }

    public class AddClientResultAction
    {
        public string ClientId { get; set; } = string.Empty;
        public string Vpn { get; set; } = string.Empty;
        public int[] PurposeTypes { get; set; } = new int[] { };
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public AddClientResult ClientResult { get; set; } = null!;
    }

    public class BulkUpdateClientResultAction
    {
        public string Vpn { get; set; }
        public ICollection<UpdateClientSubAction> Clients { get; set; } = new List<UpdateClientSubAction>();
        public BulkUpdateClientResult ClientResult { get; set; } = null!;
    }

    public class AddClientFailureAction
    {
        public AddClientFailureAction(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class GetAllClientsAction
    {
        public GetAllClientsAction(string vpn)
        {
            Vpn = vpn;
        }

        public string Vpn { get; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class CheckClientPartitionIsSyncedAction
    {
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class CheckClientPartitionIsSyncedResultAction
    {
        public bool IsSynced { get; set; }
    }

    public class RemoveClientAction
    {
        public string Vpn { get; set; }
        public string ClientId { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }

    public class RemoveClientResultAction
    {
        public bool IsRemoved { get; set; }
        public string Vpn { get; set; }
        public string ClientId { get; set; }
        public RemoveClientStatus Status { get; set; }
    }
}
