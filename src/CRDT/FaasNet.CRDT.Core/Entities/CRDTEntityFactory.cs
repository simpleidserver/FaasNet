using FaasNet.CRDT.Core.SerializedEntities;
using FaasNet.Peer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace FaasNet.CRDT.Core.Entities
{
    public interface ICRDTEntityFactory
    {
        CRDTEntity Build(SerializedEntity crdtEntity);
    }

    public class CRDTEntityFactory : ICRDTEntityFactory
    {
        private readonly PeerOptions _options;
        protected Dictionary<string, Func<SerializedEntity, string, CRDTEntity>> MappingCRDTEntityNameToFactory = new Dictionary<string, Func<SerializedEntity, string, CRDTEntity>>
        {
            { GCounter.NAME, BuildGCounter },
            { GSet.NAME, BuildGSet },
            { PNCounter.NAME, BuildPNCounter }
        };

        public CRDTEntityFactory(IOptions<PeerOptions> options)
        {
            _options = options.Value;
        }

        public CRDTEntity Build(SerializedEntity serializedEntity)
        {
            var crdtEntity = BuildEntity(serializedEntity, _options.Id);
            return crdtEntity;
        }

        protected CRDTEntity BuildEntity(SerializedEntity serializedEntity, string peerId)
        {
            return MappingCRDTEntityNameToFactory[serializedEntity.Type](serializedEntity, peerId);
        }

        protected static CRDTEntity BuildGCounter(SerializedEntity serializedEntity, string peerId)
        {
            ICollection<GCounterClockValue> clockVector = new List<GCounterClockValue>();
            if (!string.IsNullOrWhiteSpace(serializedEntity.Value)) clockVector = JsonSerializer.Deserialize<ICollection<GCounterClockValue>>(serializedEntity.Value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var gCounter = new GCounter(peerId, clockVector);
            return gCounter;
        }

        protected static CRDTEntity BuildGSet(SerializedEntity serializedEntity, string peerId)
        {
            ICollection<GSetClockValue> clockVector = new List<GSetClockValue>();
            if (!string.IsNullOrWhiteSpace(serializedEntity.Value)) clockVector = JsonSerializer.Deserialize<ICollection<GSetClockValue>>(serializedEntity.Value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var gCounter = new GSet(peerId, clockVector);
            return gCounter;
        }

        protected static CRDTEntity BuildPNCounter(SerializedEntity serializedEntity, string peerId)
        {
            ICollection<PNCounterClockValue> clockVector = new List<PNCounterClockValue>();
            if (!string.IsNullOrWhiteSpace(serializedEntity.Value)) clockVector = JsonSerializer.Deserialize<ICollection<PNCounterClockValue>>(serializedEntity.Value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var gCounter = new PNCounter(peerId, clockVector);
            return gCounter;
        }
    }
}
