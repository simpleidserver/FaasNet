using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.EF.Stores
{
    public class EFClientStore : IClientStore
    {
        private readonly EventMeshDBContext _dbContext;

        public EFClientStore(EventMeshDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Models.Client client)
        {
            _dbContext.ClientLst.Add(client);
        }

        public async Task<IEnumerable<Models.Client>> GetAllByVpn(string name, CancellationToken cancellationToken)
        {
            await EventMeshDBContext.SemaphoreSlim.WaitAsync();
            IEnumerable<Models.Client> result = await _dbContext.ClientLst.Include(c => c.Sessions).ThenInclude(s => s.Histories)
                .Include(c => c.Sessions).ThenInclude(s => s.Topics)
                .Include(c => c.Sessions).ThenInclude(s => s.PendingCloudEvents)
                .Include(c => c.Sessions).ThenInclude(s => s.Bridges)
                .Include(c => c.Topics)
                .Where(c => c.Vpn == name).ToListAsync(cancellationToken);
            EventMeshDBContext.SemaphoreSlim.Release();
            return result;
        }

        public async Task<Models.Client> GetByBridgeSession(string clientId, string bridgeUrn, string bridgeSessionId, CancellationToken cancellationToken)
        {
            await EventMeshDBContext.SemaphoreSlim.WaitAsync();
            Models.Client result = await _dbContext.ClientLst.Include(c => c.Sessions).ThenInclude(s => s.Histories)
                .Include(c => c.Sessions).ThenInclude(s => s.Topics)
                .Include(c => c.Sessions).ThenInclude(s => s.PendingCloudEvents)
                .Include(c => c.Sessions).ThenInclude(s => s.Bridges)
                .Include(c => c.Topics)
                .FirstOrDefaultAsync(c => c.ClientId == clientId && c.Sessions.Any(s => s.Bridges.Any(b => b.Urn == bridgeUrn && b.SessionId == bridgeSessionId)), cancellationToken);
            EventMeshDBContext.SemaphoreSlim.Release();
            return result;
        }

        public async Task<Models.Client> GetByClientId(string vpn, string clientId, CancellationToken cancellationToken)
        {
            await EventMeshDBContext.SemaphoreSlim.WaitAsync();
            Models.Client result = await _dbContext.ClientLst.Include(c => c.Sessions).ThenInclude(s => s.Histories)
                .Include(c => c.Sessions).ThenInclude(s => s.Topics)
                .Include(c => c.Sessions).ThenInclude(s => s.PendingCloudEvents)
                .Include(c => c.Sessions).ThenInclude(s => s.Bridges)
                .Include(c => c.Topics)
                .FirstOrDefaultAsync(c => c.Vpn == vpn && c.ClientId == clientId, cancellationToken);
            EventMeshDBContext.SemaphoreSlim.Release();
            return result;
        }

        public async Task<Models.Client> GetById(string id, CancellationToken cancellationToken)
        {
            await EventMeshDBContext.SemaphoreSlim.WaitAsync();
            Models.Client result = await _dbContext.ClientLst.Include(c => c.Sessions).ThenInclude(s => s.Histories)
                .Include(c => c.Sessions).ThenInclude(s => s.Topics)
                .Include(c => c.Sessions).ThenInclude(s => s.PendingCloudEvents)
                .Include(c => c.Sessions).ThenInclude(s => s.Bridges)
                .Include(c => c.Topics)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            EventMeshDBContext.SemaphoreSlim.Release();
            return result;
        }

        public async Task<Models.Client> GetBySession(string clientId, string clientSessionId, CancellationToken cancellationToken)
        {
            await EventMeshDBContext.SemaphoreSlim.WaitAsync();
            Models.Client result = await _dbContext.ClientLst.Include(c => c.Sessions).ThenInclude(s => s.Histories)
                .Include(c => c.Sessions).ThenInclude(s => s.Topics)
                .Include(c => c.Sessions).ThenInclude(s => s.PendingCloudEvents)
                .Include(c => c.Sessions).ThenInclude(s => s.Bridges)
                .Include(c => c.Topics)
                .FirstOrDefaultAsync(c => c.ClientId == clientId && c.Sessions.Any(s => s.Id == clientSessionId), cancellationToken);
            EventMeshDBContext.SemaphoreSlim.Release();
            return result;
        }

        public void Remove(Models.Client client)
        {
            _dbContext.ClientLst.Remove(client);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
