using System;
using System.Collections.Generic;

namespace FaasNet.Gateway.Core.Domains
{
    public class EventMeshServerAggregate
    {
        public EventMeshServerAggregate()
        {
            Bridges = new List<EventMeshServerAggregate>();
        }

        public string Urn { get; set; }
        public int Port { get; set; }
        public string CountryIsoCode { get; set; }
        public string PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreateDateTime { get; set; }
        public ICollection<EventMeshServerAggregate> Bridges { get; set; }

        public static EventMeshServerAggregate Create(string urn, int port, string countryIsoCode, string postalCode, double? latitude, double? longitude)
        {
            return new EventMeshServerAggregate
            {
                CountryIsoCode = countryIsoCode,
                CreateDateTime = DateTime.UtcNow,
                Latitude = latitude,
                Longitude = longitude,
                Port = port,
                PostalCode = postalCode,
                Urn = urn 
            };
        }
    }
}
