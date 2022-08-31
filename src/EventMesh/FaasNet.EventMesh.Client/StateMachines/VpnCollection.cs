using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.StateMachines
{
    public class VpnCollection : IStateMachine
    {
        public VpnCollection()
        {
            Values = new List<VpnRecord>();
        }

        public ICollection<VpnRecord> Values { get; set; }

        public void Apply(ICommand cmd)
        {
            switch(cmd)
            {
                case AddVpnRecordCommand addVpn:
                    Values.Add(addVpn.Record);
                    break;
                case UpdateVpnRecordCommand updateVpn:
                    var record = Values.Single(r => r.Id == updateVpn.Id);
                    record.Description = updateVpn.Description;
                    break;
                case RemoveVpnRecordCommand removeVpn:
                    Values.Remove(Values.Single(r => r.Id == removeVpn.Id));
                    break;
            }
        }

        public void Deserialize(ReadBufferContext context)
        {
            var result = new List<VpnRecord>();
            var nb = context.NextInt();
            for (var i = 0; i < nb; i++)
            {
                var ns = context.NextString();
                var entityRecord = new VpnRecord();
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
