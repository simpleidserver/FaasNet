using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface ISnapshotStore
    {
        IEnumerable<Snapshot> GetAll();
        Snapshot Get(string stateMachineId);
        void Update(Snapshot snapshot);
    }

    public class Snapshot
    {
        public long Index { get; set; }
        public long Term { get; set; }
        public string StateMachineId { get; set; }
        public byte[] StateMachine { get; set; }

        public byte[] Serialize()
        {
            var context = new WriteBufferContext();
            context.WriteLong(Index).WriteLong(Term).WriteString(StateMachineId).WriteByteArray(StateMachine);
            return context.Buffer.ToArray();
        }

        public static Snapshot Deserialize(byte[] buffer)
        {
            var readBufferContext = new ReadBufferContext(buffer);
            return new Snapshot
            {
                Index = readBufferContext.NextLong(),
                Term = readBufferContext.NextLong(),
                StateMachineId = readBufferContext.NextString(),
                StateMachine = readBufferContext.NextByteArray()
            };
        }
    }

    public class InMemorySnapshotStore : ISnapshotStore
    {
        private readonly ICollection<Snapshot> _snapshots;

        public InMemorySnapshotStore()
        {
            _snapshots = new List<Snapshot>();
        }

        public IEnumerable<Snapshot> GetAll()
        {
            return _snapshots;
        }

        public Snapshot Get(string stateMachineId)
        {
            return _snapshots.FirstOrDefault(s => s.StateMachineId == stateMachineId);
        }

        public void Update(Snapshot snapshot)
        {
            var result = _snapshots.FirstOrDefault(s => s.StateMachineId == snapshot.StateMachineId);
            if (result != null) _snapshots.Remove(result);
            _snapshots.Add(snapshot);
        }
    }
}
