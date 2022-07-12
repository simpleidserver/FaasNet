using FaasNet.CRDT.Core;
using FaasNet.CRDT.Core.Entities;
using FaasNet.CRDT.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace FaasNet.Peer
{
    public static class PeerHostFactoryExtensions
    {
        public static PeerHostFactory AddCRDTProtocol(this PeerHostFactory peerHostFactory, ConcurrentBag<SerializedEntity> entities = null)
        {
            peerHostFactory.Services.AddTransient<IProtocolHandler, CRDTProtocolHandler>();
            peerHostFactory.Services.AddTransient<IEntityFactory, EntityFactory>();
            if (entities == null) peerHostFactory.Services.AddSingleton<IEntityStore, InMemoryEntityStore>();
            else peerHostFactory.Services.AddSingleton<IEntityStore>(new InMemoryEntityStore(entities));
            return peerHostFactory;
        }
    }
}
