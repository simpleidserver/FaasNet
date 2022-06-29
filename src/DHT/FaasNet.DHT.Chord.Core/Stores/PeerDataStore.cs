using System.Collections.Generic;
using System.Linq;

namespace FaasNet.DHT.Chord.Core.Stores
{
    public interface IPeerDataStore
    {
        string Get(long id);
        void Add(long id, string value);
        ICollection<PeerDataRecord> GetAll();
        void Remove(PeerDataRecord record);
    }

    public class PeerDataStore : IPeerDataStore
    {
        private readonly ICollection<PeerDataRecord> _records;

        public PeerDataStore()
        {
            _records = new List<PeerDataRecord>();
        }

        public string Get(long id)
        {
            return _records.First(r => r.Id == id).Value;
        }

        public void Add(long id, string value)
        {
            _records.Add(new PeerDataRecord { Id = id, Value = value });
        }

        public void Remove(PeerDataRecord record)
        {
            _records.Remove(_records.First(r => r.Id == r.Id));
        }

        public ICollection<PeerDataRecord> GetAll()
        {
            return _records.Select(r => new PeerDataRecord { Id = r.Id, Value = r.Value }).ToList();
        }
    }

    public class PeerDataRecord
    {
        public long Id { get; set; }
        public string Value { get; set; }
    }
}
