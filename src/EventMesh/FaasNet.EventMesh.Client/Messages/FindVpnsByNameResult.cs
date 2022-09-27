﻿using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class FindVpnsByNameResult : BaseEventMeshPackage
    {
        public FindVpnsByNameResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.FIND_VPNS_BY_NAME_RESPONSE;
        public IEnumerable<string> Content { get; set; } = new List<string>();

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(Content.Count());
            foreach (var record in Content) context.WriteString(record);
        }

        public FindVpnsByNameResult Extract(ReadBufferContext context)
        {
            var nb = context.NextInt();
            var content = new List<string>();
            for(var i = 0; i < nb; i++)
                content.Add(context.NextString());
            Content = content;
            return this;
        }
    }
}
