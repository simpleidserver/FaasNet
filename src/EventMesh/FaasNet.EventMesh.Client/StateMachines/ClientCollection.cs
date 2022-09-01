using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.StateMachines
{
    public class ClientCollection : IStateMachine
    {
        public ClientCollection()
        {
            Values = new List<ClientRecord>();
        }

        public ICollection<ClientRecord> Values { get; set; }

        public void Apply(ICommand cmd)
        {
            switch (cmd)
            {
                case AddClientRecordCommand addClient:
                    Values.Add(addClient.Record);
                    break;
                case RemoveClientRecordCommand removeClient:
                    var cl = Values.Single(v => v.Id == removeClient.Id);
                    break;
            }
        }

        public void Deserialize(ReadBufferContext context)
        {
            var result = new List<ClientRecord>();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var entityRecord = new ClientRecord();
                entityRecord.Deserialize(context);
                result.Add(entityRecord);
            }

            Values = result;
        }

        public byte[] Serialize()
        {
            var context = new WriteBufferContext();
            context.WriteInteger(Values.Count);
            foreach (var value in Values) value.Serialize(context);
            return context.Buffer.ToArray();
        }
    }
}
