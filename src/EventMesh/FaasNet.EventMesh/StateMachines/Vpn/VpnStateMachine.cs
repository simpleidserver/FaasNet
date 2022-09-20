using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Vpn
{
    public class VpnStateMachine : IStateMachine
    {
        private readonly IVpnStateMachineStore _store;

        public VpnStateMachine(IVpnStateMachineStore store)
        {
            _store = store;
        }

        public async Task Apply(ICommand cmd, CancellationToken cancellationToken)
        {
            switch(cmd)
            {
                case AddVpnCommand addVpn:
                    var existingVpn = await _store.Get(addVpn.Id, cancellationToken);
                    if (existingVpn != null) return;
                    _store.Add(new VpnRecord 
                    {   CreateDateTime = DateTime.UtcNow, 
                        UpdateDateTime = DateTime.UtcNow, 
                        Description = addVpn.Description, 
                        Name = addVpn.Id 
                    });
                    break;
            }
        }

        public Task BulkUpload(IEnumerable<IRecord> records, CancellationToken cancellationToken)
        {
            return _store.BulkUpload(records.Cast<VpnRecord>(), cancellationToken);
        }

        public async Task Commit(CancellationToken cancellationToken)
        {
            await _store.SaveChanges(cancellationToken);
        }

        public async Task<IQueryResult> Query(IQuery query, CancellationToken cancellationToken)
        {
            switch(query)
            {
                case GetVpnQuery getVpn:
                    var existingVpn = await _store.Get(getVpn.Id, cancellationToken);
                    if (existingVpn == null) return new GetVpnQueryResult();
                    return new GetVpnQueryResult(Transform(existingVpn));
                case GetAllVpnQuery getAllVpnQuery:
                    var res = await _store.Find(getAllVpnQuery.Filter, cancellationToken);
                    return new GenericSearchQueryResult<VpnQueryResult>
                    {
                        TotalPages = res.TotalPages,
                        TotalRecords = res.TotalRecords,
                        Records = res.Records.Select(r => Transform(r))
                    };
            }

            return null;
        }

        public IEnumerable<(IEnumerable<IEnumerable<byte>>, int)> Snapshot(int nbRecords)
        {
            var name = typeof(VpnRecord).AssemblyQualifiedName;
            foreach (var records in _store.GetAll(nbRecords))
            {
                yield return (records.Item1.Select(r =>
                {
                    var ctx = new WriteBufferContext();
                    ctx.WriteString(name);
                    r.Serialize(ctx);
                    return (IEnumerable<byte>)ctx.Buffer;
                }), records.Item2);
            }
        }

        public Task Truncate(CancellationToken cancellationToken)
        {
            return _store.Truncate(cancellationToken);
        }

        private static VpnQueryResult Transform(VpnRecord vpn)
        {
            return new VpnQueryResult
            {
                Name = vpn.Name,
                Description = vpn.Description,
                CreateDateTime = vpn.CreateDateTime,
                UpdateDateTime = vpn.UpdateDateTime
            };
        }
    }

    public class VpnRecord : IRecord
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Name = context.NextString();
            Description = context.NextString();
            CreateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
            UpdateDateTime = new DateTime(context.NextTimeSpan().Value.Ticks);
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Description);
            context.WriteTimeSpan(TimeSpan.FromTicks(CreateDateTime.Ticks));
            context.WriteTimeSpan(TimeSpan.FromTicks(UpdateDateTime.Ticks));
        }
    }
}
