﻿using FaasNet.Peer.Client;
using System;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllVpnResult : BaseEventMeshPackage
    {
        public GetAllVpnResult(string seq) : base(seq)
        {
            Vpns = new List<VpnResult>();
        }

        public ICollection<VpnResult> Vpns { get; set; }

        public override EventMeshCommands Command => EventMeshCommands.GET_ALL_VPN_RESPONSE;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(Vpns.Count);
            foreach (var vpn in Vpns) vpn.Serialize(context);
        }

        public GetAllVpnResult Extract(ReadBufferContext context)
        {
            int nb = context.NextInt();
            for (var i = 0; i < nb; i++) Vpns.Add(VpnResult.Extract(context));
            return this;
        }
    }

    public class VpnResult
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Description);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.Ticks));
            context.WriteTimeSpan(TimeSpan.FromTicks(UpdateDateTime.Ticks));
        }

        public static VpnResult Extract(ReadBufferContext context)
        {
            return new VpnResult
            {
                Id = context.NextString(),
                Description = context.NextString(),
                CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks),
                UpdateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks)
            };
        }
    }
}
