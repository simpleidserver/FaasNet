using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.EventMesh.Client.StateMachines.Queue;
using FaasNet.EventMesh.Client.StateMachines.Session;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using FaasNet.RaftConsensus.Client.Messages;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.Messages
{
    public static class PackageResponseBuilder
    {
        public static BaseEventMeshPackage HeartBeat(string seq)
        {
            return new PingResult(seq);
        }

        public static BaseEventMeshPackage AddVpn(string seq)
        {
            return new AddVpnResult(seq);
        }

        public static BaseEventMeshPackage AddVpn(string seq, AddVpnErrorStatus status)
        {
            return new AddVpnResult(seq, status);
        }

        public static BaseEventMeshPackage GetAllVpn(string seq, GenericSearchQueryResult<VpnQueryResult> vpns)
        {
            return new GetAllVpnResult(seq)
            {
                Content = vpns
            };
        }

        public static BaseEventMeshPackage GetAllClient(string seq, GenericSearchQueryResult<ClientQueryResult> clients)
        {
            return new GetAllClientResult(seq)
            {
                Content = clients
            };
        }

        public static BaseEventMeshPackage AddClient(string seq, string clientId, string clientSecret, long term, long matchIndex, long lastIndex)
        {
            return new AddClientResult(seq)
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Term = term,
                MatchIndex = matchIndex,
                LastIndex = lastIndex
            };
        }

        public static BaseEventMeshPackage AddClient(string seq, AddClientErrorStatus status)
        {
            return new AddClientResult(seq, status);
        }

        public static BaseEventMeshPackage BulkUpdateClient(string seq, UpdateClientErrorStatus status)
        {
            return new BulkUpdateClientResult(seq, status);
        }

        public static BaseEventMeshPackage BulkUpdateClient(string seq)
        {
            return new BulkUpdateClientResult(seq);
        }

        public static BaseEventMeshPackage AddQueue(string seq, AddQueueStatus status = AddQueueStatus.SUCCESS)
        {
            return new AddQueueResponse(seq)
            {
                Status = status
            };
        }

        public static BaseEventMeshPackage PublishMessage(string seq, PublishMessageStatus status = PublishMessageStatus.SUCCESS)
        {
            return new PublishMessageResult(seq, status);
        }

        public static BaseEventMeshPackage PublishMessage(string seq, string id, IEnumerable<string> queueNames)
        {
            return new PublishMessageResult(seq, PublishMessageStatus.SUCCESS)
            {
                Id = id,
                QueueNames = queueNames
            };
        }

        public static BaseEventMeshPackage ReadMessage(string seq, CloudEvent message)
        {
            return new ReadMessageResult(seq)
            {
                Status = ReadMessageStatus.SUCCESS,
                Message = message
            };
        }

        public static BaseEventMeshPackage ReadMessage(string seq, ReadMessageStatus status)
        {
            return new ReadMessageResult(seq)
            {
                Status = status
            };
        }

        public static BaseEventMeshPackage Hello(string seq, HelloMessageStatus status)
        {
            return new HelloResult(seq)
            {
                Status = status
            };
        }

        public static BaseEventMeshPackage Hello(string seq, string sessionId)
        {
            return new HelloResult(seq)
            {
                SessionId = sessionId,
                Status = HelloMessageStatus.SUCCESS
            };
        }

        public static BaseEventMeshPackage GetClient(string seq, bool success, ClientQueryResult content)
        {
            return new GetClientResult(seq)
            {
                Success = success,
                Content = content
            };
        }

        public static BaseEventMeshPackage SearchSessions(string seq, GenericSearchQueryResult<SessionQueryResult> content)
        {
            return new SearchSessionsResult(seq)
            {
                Content = content
            };
        }

        public static BaseEventMeshPackage SearchQueues(string seq, GenericSearchQueryResult<QueueQueryResult> content)
        {
            return new SearchQueuesResult(seq)
            {
                Content = content
            };
        }

        public static BaseEventMeshPackage FindVpnsByName(string seq, IEnumerable<string> content)
        {
            return new FindVpnsByNameResult(seq)
            {
                Content = content
            };
        }

        public static BaseEventMeshPackage FindClientsByName(string seq, IEnumerable<string> content)
        {
            return new FindClientsByNameResult(seq)
            {
                Content = content
            };
        }

        public static BaseEventMeshPackage FindQueuesByName(string seq, IEnumerable<string> content)
        {
            return new FindQueuesByNameResult(seq)
            {
                Content = content
            };
        }

        public static BaseEventMeshPackage GetPartition(string seq, GetPartitionStatus status)
        {
            return new GetPartitionResult(seq)
            {
                Status = status
            };
        }

        public static BaseEventMeshPackage GetPartition(string seq, GetPeerStateResult state)
        {
            return new GetPartitionResult(seq)
            {
                Status = GetPartitionStatus.OK,
                State = state
            };
        }

        public static BaseEventMeshPackage RemoveClient(string seq)
        {
            return new RemoveClientResult(seq)
            {
                Status = RemoveClientStatus.OK
            };
        }

        public static BaseEventMeshPackage RemoveClient(string seq, RemoveClientStatus status)
        {
            return new RemoveClientResult(seq)
            {
                Status = status
            };
        }

        public static BaseEventMeshPackage AddEventDefinition(string seq, string eventDefId, AddEventDefinitionStatus status)
        {
            return new AddEventDefinitionResult(seq)
            {
                EventDefinitionId = eventDefId,
                Status = status
            };
        }

        public static BaseEventMeshPackage AddEventDefinition(string seq, string eventDefId, long term, long matchIndex, long lastIndex)
        {
            return new AddEventDefinitionResult(seq)
            {
                EventDefinitionId = eventDefId,
                Status = AddEventDefinitionStatus.OK,
                Term = term,
                MatchIndex = matchIndex,
                LastIndex = lastIndex
            };
        }

        public static BaseEventMeshPackage GetEventDefinition(string seq, EventDefinitionQueryResult result)
        {
            return new GetEventDefinitionResult(seq)
            {
                Status = GetEventDefinitionStatus.OK,
                Result = result
            };
        }

        public static BaseEventMeshPackage UpdateEventDefinition(string seq, UpdateEventDefinitionStatus status)
        {
            return new UpdateEventDefinitionResult(seq)
            {
                Status = status
            };
        }

        public static BaseEventMeshPackage GetEventDefinition(string seq, GetEventDefinitionStatus status)
        {
            return new GetEventDefinitionResult(seq)
            {
                Status = status
            };
        }

        public static BaseEventMeshPackage RemoveLinkEventDefinition(string seq, RemoveEventDefinitionStatus status)
        {
            return new RemoveLinkEventDefinitionResult(seq)
            {
                Status = status
            };
        }
    }
}
