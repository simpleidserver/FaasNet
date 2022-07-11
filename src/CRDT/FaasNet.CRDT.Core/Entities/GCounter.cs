using FaasNet.CRDT.Client.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CRDT.Core.Entities
{
    public class GCounter : CRDTEntity<GCounterDelta>
    {
        public static string NAME = "GCounter";
        private readonly string _replicatonId;
        private readonly Dictionary<string, long> _replicatedValues;
        private long _delta;

        public GCounter(string replicationId)
        {
            _replicatonId = replicationId;
            _replicatedValues = new Dictionary<string, long>();
        }

        public override string Name => NAME;
        public override bool HasDelta => _delta != default(long);
        public long Value => _replicatedValues.Sum(kvp => kvp.Value);

        public override GCounterDelta ResetAndGetDelta()
        {
            var result = new GCounterDelta(_delta);
            _delta = default(long);
            return result;
        }

        public override void ApplyDelta(string replicationId, GCounterDelta delta)
        {
            if (!_replicatedValues.ContainsKey(replicationId)) _replicatedValues.Add(replicationId, 0);
            _replicatedValues[replicationId] = Math.Max(_replicatedValues[replicationId], _replicatedValues[_replicatonId]);
        }

        public GCounter Increment()
        {
            if (!_replicatedValues.ContainsKey(_replicatonId)) _replicatedValues.Add(_replicatonId, 0);
            _replicatedValues[_replicatonId] = _replicatedValues[_replicatonId] + 1;
            _delta = _replicatedValues[_replicatonId];
            return this;
        }
    }
}
