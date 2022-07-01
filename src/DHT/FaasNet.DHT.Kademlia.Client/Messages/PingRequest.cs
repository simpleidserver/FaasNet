﻿namespace FaasNet.DHT.Kademlia.Client.Messages
{
    public class PingRequest : BasePackage
    {
        public PingRequest() : base(Commands.PING_REQUEST)
        {
        }

        public long Id { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteLong(Id);
            context.WriteString(Url);
            context.WriteInteger(Port);
        }

        public override BasePackage Extract(ReadBufferContext context)
        {
            base.Extract(context);
            Id = context.NextLong();
            Url = context.NextString();
            Port = context.NextInt();
            return this;
        }
    }
}
