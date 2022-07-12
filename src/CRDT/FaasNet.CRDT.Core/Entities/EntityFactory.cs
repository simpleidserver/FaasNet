using FaasNet.CRDT.Client.Messages.Deltas;
using FaasNet.CRDT.Core.Stores;
using FaasNet.Peer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace FaasNet.CRDT.Core.Entities
{
    public interface IEntityFactory
    {
        void ApplyDelta<T>(SerializedEntity crdtEntity, T request, string peerId) where T : BaseEntityDelta;
    }

    public class EntityFactory : IEntityFactory
    {
        private readonly PeerOptions _options;
        protected Dictionary<string, Func<SerializedEntity, string, CRDTEntity>> MappingCRDTEntityNameToFactory = new Dictionary<string, Func<SerializedEntity, string, CRDTEntity>>
        {
            { GCounter.NAME, BuildGCounter }
        };

        public EntityFactory(IOptions<PeerOptions> options)
        {
            _options = options.Value;
        }

        public void ApplyDelta<T>(SerializedEntity serializedEntity, T request, string peerId) where T : BaseEntityDelta
        {
            var crdtEntity = BuildEntity(serializedEntity, _options.PeerId);
            crdtEntity.ApplyDelta(peerId, request);
            string s = "";
            // Il faut incrémenter.
            // Récupérer la dernière entité.Si l'entité n'existe pas alors on va la créer sur les autres noeuds.
            // Si l'entité n'existe pas alors il faut la créer.
            // Si l'entité existe alors le serveur cible retourne : la dernière version connue et envoie la suite.
            // Si l'entité existe déjà 
        }

        protected CRDTEntity BuildEntity(SerializedEntity serializedEntity, string peerId)
        {
            return MappingCRDTEntityNameToFactory[serializedEntity.Type](serializedEntity, peerId);
        }

        protected static CRDTEntity BuildGCounter(SerializedEntity serializedEntity, string peerId)
        {
            var replicatedValues = new Dictionary<string, long>();
            if (!string.IsNullOrWhiteSpace(serializedEntity.Value)) replicatedValues = JsonSerializer.Deserialize<Dictionary<string, long>>(serializedEntity.Value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var gCounter = new GCounter(peerId, replicatedValues);
            return gCounter;
        }
    }
}
