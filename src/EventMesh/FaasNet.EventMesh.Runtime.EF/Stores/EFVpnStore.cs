using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.EF.Stores
{
    public class EFVpnStore : IVpnStore
    {
        private readonly EventMeshDBContext _dbContext;

        public EFVpnStore(EventMeshDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Vpn vpn)
        {
            _dbContext.VpnLst.Add(vpn);
        }

        public Task<Vpn> Get(string name, CancellationToken cancellationToken)
        {
            lock (EventMeshDBContext.Lock)
                return _dbContext.VpnLst
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.Histories)
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.Topics)
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.PendingCloudEvents)
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.Bridges)
                    .Include(c => c.Clients).ThenInclude(c => c.Topics)
                    .Include(c => c.BridgeServers)
                    .Include(c => c.ApplicationDomains)
                    .FirstOrDefaultAsync(v => v.Name == name, cancellationToken);
        }

        public Task<Vpn> Get(string clientId, string sessionId, CancellationToken cancellationToken)
        {
            lock (EventMeshDBContext.Lock)
                return _dbContext.VpnLst
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.Histories)
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.Topics)
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.PendingCloudEvents)
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.Bridges)
                    .Include(c => c.Clients).ThenInclude(c => c.Topics)
                    .Include(c => c.BridgeServers)
                    .Include(c => c.ApplicationDomains)
                    .FirstOrDefaultAsync(v => v.Clients.Any(c => c.ClientId == clientId && c.Sessions.Any(s => s.Id == sessionId)), cancellationToken);
        }

        public Task<Vpn> Get(string clientId, string urn, string sessionId, CancellationToken cancellationToken)
        {
            lock (EventMeshDBContext.Lock)
                return _dbContext.VpnLst
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.Histories)
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.Topics)
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.PendingCloudEvents)
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.Bridges)
                    .Include(c => c.Clients).ThenInclude(c => c.Topics)
                    .Include(c => c.BridgeServers)
                    .Include(c => c.ApplicationDomains)
                    .FirstOrDefaultAsync(v => v.Clients.Any(c => c.ClientId == clientId && c.Sessions.Any(s => s.Bridges.Any(b => b.Urn == urn && b.SessionId == sessionId))), cancellationToken);
        }

        public Task<IEnumerable<Vpn>> GetAll(CancellationToken cancellationToken)
        {
            lock (EventMeshDBContext.Lock)
            {
                IEnumerable<Vpn> result = _dbContext.VpnLst
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.Histories)
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.Topics)
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.PendingCloudEvents)
                    .Include(c => c.Clients).ThenInclude(c => c.Sessions).ThenInclude(c => c.Bridges)
                    .Include(c => c.Clients).ThenInclude(c => c.Topics)
                    .Include(c => c.BridgeServers)
                    .Include(c => c.ApplicationDomains)
                    .ToList();
                return Task.FromResult(result);
            }
        }

        public void Delete(Vpn vpn)
        {
            _dbContext.VpnLst.Remove(vpn);
        }

        public void Update(Vpn vpn)
        {
            _dbContext.VpnLst.Update(vpn);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
