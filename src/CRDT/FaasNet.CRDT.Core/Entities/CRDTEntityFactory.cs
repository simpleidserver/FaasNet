using FaasNet.CRDT.Client.Messages.Deltas;
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
            { GCounter.NAME, BuildGCounter }
        };

        public CRDTEntityFactory(IOptions<PeerOptions> options)
        {
            _options = options.Value;
        }

        public CRDTEntity Build(SerializedEntity serializedEntity)
        {
            var crdtEntity = BuildEntity(serializedEntity, _options.PeerId);
            return crdtEntity;
        }

        protected CRDTEntity BuildEntity(SerializedEntity serializedEntity, string peerId)
        {
            return MappingCRDTEntityNameToFactory[serializedEntity.Type](serializedEntity, peerId);
        }

        protected static CRDTEntity BuildGCounter(SerializedEntity serializedEntity, string peerId)
        {
            ICollection<GCounterClockValue> replicatedValues = new List<GCounterClockValue>();
            if (!string.IsNullOrWhiteSpace(serializedEntity.Value)) replicatedValues = JsonSerializer.Deserialize<ICollection<GCounterClockValue>>(serializedEntity.Value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var gCounter = new GCounter(peerId, replicatedValues);
            return gCounter;
        }
    }
}
