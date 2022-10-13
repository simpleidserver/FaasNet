using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddEventDefinitionRequest addEventDefinition, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(VPN_PARTITION_KEY, new GetVpnQuery { Id = addEventDefinition.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.AddEventDefinition(addEventDefinition.Seq, addEventDefinition.Id, AddEventDefinitionStatus.UNKNOWN_VPN);
            var source = await Query<GetClientQueryResult>(CLIENT_PARTITION_KEY, new GetClientQuery { Id = addEventDefinition.Source, Vpn = addEventDefinition.Vpn }, cancellationToken);
            if (!source.Success) return PackageResponseBuilder.AddEventDefinition(addEventDefinition.Seq, addEventDefinition.Id, AddEventDefinitionStatus.UNKNOWN_SOURCE);
            var target = await Query<GetClientQueryResult>(CLIENT_PARTITION_KEY, new GetClientQuery { Id = addEventDefinition.Target, Vpn = addEventDefinition.Vpn }, cancellationToken);
            if (!target.Success) return PackageResponseBuilder.AddEventDefinition(addEventDefinition.Seq, addEventDefinition.Id, AddEventDefinitionStatus.UNKNOWN_TARGET);
            var eventDef = await Query<GetEventDefinitionQueryResult>(EVENTDEFINITION_PARTITION_KEY, new GetEventDefinitionQuery { Id = addEventDefinition.Id, Vpn = addEventDefinition.Vpn }, cancellationToken);
            if (eventDef.Success) return PackageResponseBuilder.AddEventDefinition(addEventDefinition.Seq, addEventDefinition.Id, AddEventDefinitionStatus.EXISTING_EVENTDEFINITION);
            var bulkUpdateClientCommand = new BulkUpdateClientCommand
            {
                Vpn = addEventDefinition.Vpn,
                Clients = new List<UpdateClient>
                {
                    Build(source, addEventDefinition.Id, addEventDefinition.Target),
                    Build(target)
                }
            };
            await Send(CLIENT_PARTITION_KEY, bulkUpdateClientCommand, cancellationToken);
            var addEventDefCmd = new AddEventDefinitionCommand { Id = addEventDefinition.Id, JsonSchema = addEventDefinition.JsonSchema, Source = addEventDefinition.Source, Target = addEventDefinition.Target, Vpn = addEventDefinition.Vpn };
            var result = await Send(EVENTDEFINITION_PARTITION_KEY, addEventDefCmd, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.AddEventDefinition(addEventDefinition.Seq, addEventDefinition.Id, AddEventDefinitionStatus.NOLEADER);
            return PackageResponseBuilder.AddEventDefinition(addEventDefinition.Seq, addEventDefinition.Id, result.Term, result.MatchIndex, result.LastIndex);
        }

        UpdateClient Build(GetClientQueryResult client, string eventId = null, string target = null)
        {
            var targets = client.Client.Targets;
            if (!string.IsNullOrWhiteSpace(target)) targets.Add(new ClientTargetResult
            {
                EventId = eventId,
                Target = target
            });
            return new UpdateClient
            {
                CoordinateX = client.Client.CoordinateX,
                CoordinateY = client.Client.CoordinateY,
                Id = client.Client.Id,
                Targets = targets
            };
        }
    }
}
