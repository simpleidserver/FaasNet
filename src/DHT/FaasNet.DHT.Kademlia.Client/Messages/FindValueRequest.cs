﻿namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class FindValueRequest : BasePackage
    {
        public FindValueRequest() : base(Commands.FIND_VALUE_REQUEST)
        {
        }

        public long Key { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteLong(Key);
        }

        public override BasePackage Extract(ReadBufferContext context)
        {
            base.Extract(context);
            Key = context.NextLong();
            return this;
        }
    }
}
