﻿using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages.Deltas
{
    public class EntityDeltaTypes : BaseEnumeration
    {
        public static EntityDeltaTypes GCounter = new EntityDeltaTypes(0, "GCOUNTER");
        public static EntityDeltaTypes GSet = new EntityDeltaTypes(1, "GSET");
        public static EntityDeltaTypes PNCounter = new EntityDeltaTypes(2, "PNCounter");

        public EntityDeltaTypes(int code)
        {
            Init<EntityDeltaTypes>(code);
        }

        protected EntityDeltaTypes(int code, string name) : base(code, name)
        {
        }

        public static EntityDeltaTypes Deserialize(ReadBufferContext context)
        {
            return new EntityDeltaTypes(context.NextInt());
        }
    }
}