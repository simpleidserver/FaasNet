﻿using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.StateMachines.Client
{
    public class BulkUpdateClientCommand : ICommand
    {
        public string Vpn { get; set; }
        public ICollection<UpdateClient> Clients { get; set; } = new List<UpdateClient>();

        public void Deserialize(ReadBufferContext context)
        {
            Vpn = context.NextString();
            var nbTargets = context.NextInt();
            for (var i = 0; i < nbTargets; i++)
            {
                var cl = new UpdateClient();
                cl.Deserialize(context);
                Clients.Add(cl);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteInteger(Clients.Count);
            foreach (var client in Clients) client.Serialize(context);
        }
    }

    public class UpdateClient
    {
        public string Id { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public ICollection<ClientTargetResult> Targets { get; set; } = new List<ClientTargetResult>();

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            CoordinateX = context.NextDouble();
            CoordinateY = context.NextDouble();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var target = new ClientTargetResult();
                target.Deserialize(context);
                Targets.Add(target);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteDouble(CoordinateX);
            context.WriteDouble(CoordinateY);
            context.WriteInteger((int)Targets.Count);
            foreach (var target in Targets) target.Serialize(context);
        }
    }
}
