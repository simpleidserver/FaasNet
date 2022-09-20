using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.StateMachines
{
    public class GenericSearchQueryResult<T> : IQueryResult where T : ISerializable
    {
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Records { get; set; } = new List<T>();

        public void Deserialize(ReadBufferContext context)
        {
            TotalRecords = context.NextInt();
            TotalPages = context.NextInt();
            var nb = context.NextInt();
            var records = new List<T>();
            for(var i = 0; i < nb; i++)
            {
                var record = Activator.CreateInstance<T>();
                record.Deserialize(context);
                records.Add(record);
            }

            Records = records;
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteInteger(TotalPages);
            context.WriteInteger(TotalPages);
            context.WriteInteger(Records.Count());
            foreach (var record in Records) record.Serialize(context);
        }
    }
}
