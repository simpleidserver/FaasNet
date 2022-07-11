using FaasNet.CRDT.Client.Messages;
using FaasNet.CRDT.Core.Stores;
using System;
using System.Collections.Generic;

namespace FaasNet.CRDT.Core.Entities
{
    public interface IEntityFactory
    {
        void ApplyDelta<T>(SerializedEntity crdtEntity, T request) where T : IEntityDelta;
    }

    public class EntityFactory : IEntityFactory
    {
        protected Dictionary<string, Func<SerializedEntity, object>> MappingCRDTEntityNameToFactory = new Dictionary<string, Func<SerializedEntity, object>>
        {
            { GCounter.NAME, BuildGCounter }
        };

        public void ApplyDelta<T>(SerializedEntity crdtEntity, T request) where T : IEntityDelta
        {
            var obj = BuildEntity(crdtEntity);

            // Il faut incrémenter.
            // Récupérer la dernière entité.Si l'entité n'existe pas alors on va la créer sur les autres noeuds.
            // Si l'entité n'existe pas alors il faut la créer.
            // Si l'entité existe alors le serveur cible retourne : la dernière version connue et envoie la suite.
            // Si l'entité existe déjà 
        }

        protected object BuildEntity(SerializedEntity serializedEntity)
        {
            return MappingCRDTEntityNameToFactory[serializedEntity.Type](serializedEntity);
        }

        protected static object BuildGCounter(SerializedEntity serializedEntity)
        {

            return null;
        }
    }
}
