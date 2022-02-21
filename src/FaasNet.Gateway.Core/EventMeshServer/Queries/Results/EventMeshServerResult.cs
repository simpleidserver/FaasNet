using FaasNet.Gateway.Core.Domains;
using System;

namespace FaasNet.Gateway.Core.EventMeshServer.Queries.Results
{
    public class EventMeshServerResult
    {
        public string Urn { get; set; }
        public int Port { get; set; }
        public string CountryIsoCode { get; set; }
        public string PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreateDateTime { get; set; }

        public static EventMeshServerResult ToDto(EventMeshServerAggregate eventMeshServer)
        {
            return new EventMeshServerResult
            {
                CountryIsoCode = eventMeshServer.CountryIsoCode,
                CreateDateTime = eventMeshServer.CreateDateTime,
                Latitude = eventMeshServer.Latitude,
                Longitude = eventMeshServer.Longitude,
                Port = eventMeshServer.Port,
                PostalCode = eventMeshServer.PostalCode,
                Urn = eventMeshServer.Urn
            };
        }
    }
}
