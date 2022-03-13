using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Gateway.Core.Domains
{
    public class EventMeshServerAggregate
    {
        public EventMeshServerAggregate()
        {
            Bridges = new List<EventMeshServerBridge>();
        }

        public string Urn { get; set; }
        public int Port { get; set; }
        public string CountryIsoCode { get; set; }
        public string PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreateDateTime { get; set; }
        public ICollection<EventMeshServerBridge> Bridges { get; set; }

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

        public void AddBridge(string urn, int port)
        {
            if(Bridges.Any(b => b.Urn == urn && b.Port == port))
            {
                throw new DomainException(ErrorCodes.BridgeAlreadyExists, Global.BridgeAlreadyExists);
            }

            Bridges.Add(new EventMeshServerBridge
            {
                Port = port,
                Urn = urn
            });
        }
    }
}
