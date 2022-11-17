using FaasNet.Common.Extensions;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Subscriptions
{
    public interface ISubscriptionStateMachineStore : IStateMachineRecordStore<SubscriptionRecord>
    {
        Task<IEnumerable<SubscriptionRecord>> GetAll(string topicFilter, string vpn, CancellationToken cancellationToken);
        Task<SubscriptionRecord> Get(string clientId, string eventId, string vpn, CancellationToken cancellationToken);
        void Delete(SubscriptionRecord record);
    }

    public class SubscriptionStateMachineStore : ISubscriptionStateMachineStore
    {
        private ConcurrentBag<SubscriptionRecord> _subscriptions = new ConcurrentBag<SubscriptionRecord>();

        public void Add(SubscriptionRecord record)
        {
            _subscriptions.Add(record);
        }

        public Task BulkUpload(IEnumerable<SubscriptionRecord> records, CancellationToken cancellationToken)
        {
            _subscriptions = new ConcurrentBag<SubscriptionRecord>(records);
            return Task.CompletedTask;
        }

        public void Delete(SubscriptionRecord record)
        {
            _subscriptions.Remove(record);
        }

        public Task<SubscriptionRecord> Get(string key, CancellationToken cancellationToken)
        {
            return Task.FromResult(_subscriptions.FirstOrDefault(s => s.Id == key));
        }

        public Task<SubscriptionRecord> Get(string clientId, string eventId, string vpn, CancellationToken cancellationToken)
        {
            return Task.FromResult(_subscriptions.FirstOrDefault(s => s.QueueName == clientId && s.EventId == eventId && s.Vpn == vpn));
        }

        public IEnumerable<(IEnumerable<SubscriptionRecord>, int)> GetAll(int nbRecords)
        {
            int nbPages = (int)Math.Ceiling((double)_subscriptions.Count / nbRecords);
            for (var currentPage = 0; currentPage < nbPages; currentPage++)
                yield return (_subscriptions.Skip(currentPage * nbRecords).Take(nbRecords), currentPage);
        }

        public Task<IEnumerable<SubscriptionRecord>> GetAll(string topicFilter, string vpn, CancellationToken cancellationToken)
        {
            var regex = new Regex(topicFilter);
            return Task.FromResult(_subscriptions.Where(s => regex.IsMatch(s.Topic) && s.Vpn == vpn));
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task Truncate(CancellationToken cancellationToken)
        {
            _subscriptions = new ConcurrentBag<SubscriptionRecord>();
            return Task.CompletedTask;
        }

        public void Update(SubscriptionRecord record)
        {
        }
    }
}
