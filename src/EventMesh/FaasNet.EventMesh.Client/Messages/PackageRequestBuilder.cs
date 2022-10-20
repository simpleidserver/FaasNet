using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaasNet.EventMesh.Client.Messages
{
    public static class PackageRequestBuilder
    {
        public static BaseEventMeshPackage HeartBeat()
        {
            return new PingRequest(GenerateRandomSeq());
        }
        public static BaseEventMeshPackage AddVpn(string vpn, string description)
        {
            return new AddVpnRequest(GenerateRandomSeq(), vpn, description);
        }

        public static BaseEventMeshPackage GetAllVpn(FilterQuery filter)
        {
            return new GetAllVpnRequest(GenerateRandomSeq())
            {
                Filter = filter
            };
        }

        public static BaseEventMeshPackage AddClient(string clientId, string vpn, ICollection<ClientPurposeTypes> purposes)
        {
            return new AddClientRequest(GenerateRandomSeq(), clientId, vpn, purposes);
        }

        public static BaseEventMeshPackage GetAllClient(FilterQuery filter)
        {
            return new GetAllClientRequest(GenerateRandomSeq())
            {
                Filter = filter
            };
        }

        public static BaseEventMeshPackage GetAllApplicationDomains(FilterQuery filter)
        {
            return new GetAllApplicationDomainsRequest(GenerateRandomSeq())
            {
                Filter = filter
            };
        }

        public static BaseEventMeshPackage AddQueue(string vpn, string name, string topicFilter)
        {
            return new AddQueueRequest(GenerateRandomSeq())
            {
                Vpn = vpn,
                QueueName = name,
                TopicFilter = topicFilter
            };
        }

        public static BaseEventMeshPackage PublishMessage(string topic, string sessionId, CloudEvent cloudEvt)
        {
            return new PublishMessageRequest(GenerateRandomSeq())
            {
                Topic = topic,
                SessionId = sessionId,
                CloudEvent = cloudEvt
            };
        }

        public static BaseEventMeshPackage Hello(string clientId, string vpn, string clientSecret, string queueName, ClientPurposeTypes purpose)
        {
            return new HelloRequest(GenerateRandomSeq())
            {
                ClientId = clientId,
                Vpn = vpn,
                ClientSecret = clientSecret,
                QueueName = queueName,
                Purpose = purpose
            };
        }

        public static BaseEventMeshPackage ReadMessage(int offset, string sessionId)
        {
            return new ReadMessageRequest(GenerateRandomSeq())
            {
                Offset = offset,
                SessionId = sessionId
            };
        }

        public static BaseEventMeshPackage GetClient(string clientId, string vpn)
        {
            return new GetClientRequest(GenerateRandomSeq())
            {
                ClientId = clientId,
                Vpn = vpn
            };
        }

        public static BaseEventMeshPackage SearchSessions(string clientId, string vpn, FilterQuery filter)
        {
            return new SearchSessionsRequest(GenerateRandomSeq())
            {
                ClientId = clientId,
                Vpn = vpn,
                Filter = filter
            };
        }

        public static BaseEventMeshPackage SearchQueues(FilterQuery filter)
        {
            return new SearchQueuesRequest(GenerateRandomSeq())
            {
                Filter = filter
            };
        }

        public static BaseEventMeshPackage FindVpnsByName(string name)
        {
            return new FindVpnsByNameRequest(GenerateRandomSeq())
            {
                Name = name
            };
        }

        public static BaseEventMeshPackage FindClientsByName(string name)
        {
            return new FindClientsByNameRequest(GenerateRandomSeq())
            {
                Name = name
            };
        }

        public static BaseEventMeshPackage FindQueuesByName(string name)
        {
            return new FindQueuesByNameRequest(GenerateRandomSeq())
            {
                Name = name
            };
        }

        public static BaseEventMeshPackage GetPartition(string partition)
        {
            return new GetPartitionRequest(GenerateRandomSeq())
            {
                Partition = partition
            };
        }

        public static BaseEventMeshPackage AddEventDefinition(string id, string vpn, string jsonSchema, string description)
        {
            return new AddEventDefinitionRequest(GenerateRandomSeq())
            {
                Id = id,
                JsonSchema = jsonSchema,
                Vpn = vpn,
                Description = description
            };
        }

        public static BaseEventMeshPackage GetEventDefinition(string id, string vpn)
        {
            return new GetEventDefinitionRequest(GenerateRandomSeq())
            {
                Vpn = vpn,
                Id = id
            };
        }

        public static BaseEventMeshPackage UpdateEventDefinition(string id, string vpn, string jsonSchema)
        {
            return new UpdateEventDefinitionRequest(GenerateRandomSeq())
            {
                Id = id,
                Vpn = vpn,
                JsonSchema = jsonSchema
            };
        }

        public static BaseEventMeshPackage RemoveLinkApplicationDomain(string name, string vpn, string source, string target, string eventId)
        {
            return new RemoveLinkApplicationDomainRequest(GenerateRandomSeq())
            {
                Name = name,
                Vpn = vpn,
                Source = source,
                Target = target,
                EventId = eventId
            };
        }

        public static BaseEventMeshPackage AddLinkApplicationDomain(string name, string vpn, string source, string target, string eventId)
        {
            return new AddLinkApplicationDomainRequest(GenerateRandomSeq())
            {
                Name = name,
                Vpn = vpn,
                Source = source,
                Target = target,
                EventId = eventId
            };
        }

        public static BaseEventMeshPackage AddApplicationDomain(string name, string vpn, string description, string rootTopic)
        {
            return new AddApplicationDomainRequest(GenerateRandomSeq())
            {
                Name = name,
                Vpn = vpn,
                Description = description,
                RootTopic = rootTopic
            };
        }

        public static BaseEventMeshPackage UpdateApplicationDomainCoordinates(string name, string vpn, ICollection<ApplicationDomainCoordinate> coordinates)
        {
            return new UpdateApplicationDomainCoordinatesRequest(GenerateRandomSeq())
            {
                Name = name,
                Vpn = vpn,
                Coordinates = coordinates
            };
        }

        public static BaseEventMeshPackage GetApplicationDomain(string name, string vpn)
        {
            return new GetApplicationDomainRequest(GenerateRandomSeq())
            {
                Name = name,
                Vpn = vpn
            };
        }

        public static BaseEventMeshPackage AddApplicationDomainElement(string name, string vpn, string elementId, double coordinateX, double coordinateY)
        {
            return new AddElementApplicationDomainRequest(GenerateRandomSeq())
            {
                Name = name,
                Vpn = vpn,
                ElementId = elementId,
                CoordinateX = coordinateX,
                CoordinateY = coordinateY
            };
        }

        public static BaseEventMeshPackage RemoveApplicationDomainElement(string name, string vpn, string elementId)
        {
            return new RemoveElementApplicationDomainRequest(GenerateRandomSeq())
            {
                Name = name,
                Vpn = vpn,
                ElementId = elementId
            };
        }

        public static BaseEventMeshPackage GetAllEventDefs(FilterQuery filter)
        {
            return new GetAllEventDefsRequest(GenerateRandomSeq())
            {
                Filter = filter
            };
        }

        private static string GenerateRandomSeq()
        {
            var builder = new StringBuilder(10);
            var rnd = new Random();
            for (int i = 0; i < 10; i++)
                builder.Append((char)rnd.Next(48, 57));
            return builder.ToString();
        }
    }
}
