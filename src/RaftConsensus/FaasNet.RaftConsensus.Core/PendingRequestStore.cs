using FaasNet.RaftConsensus.Client.Messages;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.RaftConsensus.Core
{
    public interface IPendingRequestStore
    {
        void Add(AppendEntryRequest request);
        ICollection<AppendEntryRequest> GetAll();
        void Clean();
    }

    public class PendingRequestStore : IPendingRequestStore
    {
        private readonly ConcurrentBag<AppendEntryRequest> _pendingRequestLst;

        public PendingRequestStore()
        {
            _pendingRequestLst = new ConcurrentBag<AppendEntryRequest>();
        }

        public void Add(AppendEntryRequest request)
        {
            _pendingRequestLst.Add(request);
        }

        public ICollection<AppendEntryRequest> GetAll()
        {
            return _pendingRequestLst.ToList();
        }

        public void Clean()
        {
            _pendingRequestLst.Clear();
        }
    }
}
