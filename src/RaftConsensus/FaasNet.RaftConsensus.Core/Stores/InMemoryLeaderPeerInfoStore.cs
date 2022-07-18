using FaasNet.Common.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface ILeaderPeerInfoStore
    {
        Task<LeaderPeerInfo> Get(string partitionId, CancellationToken cancellationToken);
        Task<ICollection<LeaderPeerInfo>> GetAll(CancellationToken cancellationToken);
        Task Update(LeaderPeerInfo leaderPeerInfo, CancellationToken cancellationToken);
    }

    public class InMemoryLeaderPeerInfoStore : ILeaderPeerInfoStore
    {
        private readonly ConcurrentBag<LeaderPeerInfo> _leaderPeerInfos;

        public InMemoryLeaderPeerInfoStore()
        {
            _leaderPeerInfos = new ConcurrentBag<LeaderPeerInfo>();
        }

        public Task<LeaderPeerInfo> Get(string partitionId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_leaderPeerInfos.FirstOrDefault(p => p.PartitionId == partitionId));
        }

        public Task<ICollection<LeaderPeerInfo>> GetAll(CancellationToken cancellationToken)
        {
            ICollection<LeaderPeerInfo> result = _leaderPeerInfos.ToList();
            return Task.FromResult(result);
        }

        public Task Update(LeaderPeerInfo leaderPeerInfo, CancellationToken cancellationToken)
        {
            var existingLeaderPeerInfo = _leaderPeerInfos.FirstOrDefault(p => p.PartitionId == leaderPeerInfo.PartitionId);
            if (existingLeaderPeerInfo != null) _leaderPeerInfos.Remove(existingLeaderPeerInfo);
            _leaderPeerInfos.Add(leaderPeerInfo);
            return Task.CompletedTask;
        }
    }

    public class LeaderPeerInfo
    {
        public LeaderPeerInfo(string partitionId, string url, int port)
        {
            PartitionId = partitionId;
            Url = url;
            Port = port;
            HeartbeatReceivedDateTime = DateTime.UtcNow;
        }

        public string PartitionId { get; private set; }
        public string Url { get; private set; }
        public int Port { get; private set; }
        public DateTime? HeartbeatReceivedDateTime { get; set; }

        public bool IsActive(int durationMS)
        {
            var currentDateTime = DateTime.UtcNow;
            return HeartbeatReceivedDateTime != null && HeartbeatReceivedDateTime.Value.AddMilliseconds(durationMS) >= currentDateTime;
        }
    }
}
