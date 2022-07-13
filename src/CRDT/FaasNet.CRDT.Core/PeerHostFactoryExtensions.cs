using FaasNet.CRDT.Core;
using FaasNet.CRDT.Core.Entities;
using FaasNet.CRDT.Core.SerializedEntities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace FaasNet.Peer
{
    public static class PeerHostFactoryExtensions
    {
        public static PeerHostFactory AddCRDTProtocol(this PeerHostFactory peerHostFactory, ConcurrentBag<SerializedEntity> entities = null, Action<CRDTProtocolOptions> options = null)
        {
            if (options == null) peerHostFactory.Services.Configure<CRDTProtocolOptions>(o => { });
            else peerHostFactory.Services.Configure(options);
            peerHostFactory.Services.AddTransient<IProtocolHandler, CRDTProtocolHandler>();
            peerHostFactory.Services.AddTransient<ICRDTEntityFactory, CRDTEntityFactory>();
            peerHostFactory.Services.AddTransient<ITimer, SyncCRDTEntitiesTimer>();
            if (entities == null) peerHostFactory.Services.AddSingleton<ISerializedEntityStore, InMemorySerializedEntityStore>();
            else peerHostFactory.Services.AddSingleton<ISerializedEntityStore>(new InMemorySerializedEntityStore(entities));
            return peerHostFactory;
        }
    }
}
