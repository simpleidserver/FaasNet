using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.RaftConsensus.Core.StateMachines
{
    public class GCollection : IStateMachine
    {
        public ICollection<IEntityRecord> Values { get; set; } = new List<IEntityRecord>();

        public void Apply(ICommand command)
        {
            switch (command)
            {
                case AddStringEntityCommand addEntity:
                    Values.Add(addEntity.Record);
                    break;
                case RemoveEntityCommand removeEntity:
                    Values.Remove(Values.First(v => v.Id == removeEntity.Id));
                    break;
            }
        }

        public void Deserialize(ReadBufferContext context)
        {
            var result = new List<IEntityRecord>();
            var nb = context.NextInt();
            for(var i = 0; i < nb; i++)
            {
                var ns = context.NextString();
                var entityRecordType = Type.GetType(ns);
                var entityRecord = (IEntityRecord)Activator.CreateInstance(entityRecordType);
                entityRecord.Deserialize(context);
                result.Add(entityRecord);
            }

            Values = result;
        }

        public byte[] Serialize()
        {
            var context = new WriteBufferContext();
            context.WriteInteger(Values.Count);
            foreach(var value in Values)
            {
                context.WriteString(value.GetType().AssemblyQualifiedName);
                value.Serialize(context);
            }

            return context.Buffer.ToArray();
        }
    }
}
