using System.Collections.Generic;
using System.Linq;

namespace FaasNet.DHT.Kademlia.Core.Stores
{
    public interface IPeerDataStore
    {
        bool TryGet(long id, out string value);
        void Add(long id, string value);
        ICollection<PeerDataRecord> GetAll();
        bool TryRemove(PeerDataRecord record);
    }

    public class PeerDataStore : IPeerDataStore
    {
        private readonly ICollection<PeerDataRecord> _peerDataRecords;

        public PeerDataStore()
        {
            _peerDataRecords = new List<PeerDataRecord>();
        }

        public void Add(long id, string value)
        {
            _peerDataRecords.Add(new PeerDataRecord { Id = id, Value = value });
        }

        public bool TryGet(long id, out string value)
        {
            value = null;
            var result = _peerDataRecords.FirstOrDefault(p => p.Id == id);
            if (result == null) return false;
            value = result.Value;
            return true;
        }

        public ICollection<PeerDataRecord> GetAll()
        {
            return _peerDataRecords;
        }

        public bool TryRemove(PeerDataRecord record)
        {
            var existingRecord = _peerDataRecords.FirstOrDefault(p => p.Id == record.Id);
            if(existingRecord == null) return false;
            _peerDataRecords.Remove(existingRecord);
            return true;
        }
    }

    public class PeerDataRecord
    {
        public long Id { get; set; }
        public string Value { get; set; }
    }
}
