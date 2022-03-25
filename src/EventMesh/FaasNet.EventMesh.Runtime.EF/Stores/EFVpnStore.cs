using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<Vpn> Get(string name, CancellationToken cancellationToken)
        {
            await EventMeshDBContext.SemaphoreSlim.WaitAsync(cancellationToken);
            var result = await _dbContext.VpnLst
                    .Include(c => c.BridgeServers)
                    .FirstOrDefaultAsync(v => v.Name == name, cancellationToken);
            EventMeshDBContext.SemaphoreSlim.Release();
            return result;
        }

        public async Task<IEnumerable<Vpn>> GetAll(CancellationToken cancellationToken)
        {
            await EventMeshDBContext.SemaphoreSlim.WaitAsync(cancellationToken);
            var result = await _dbContext.VpnLst.ToListAsync(cancellationToken);
            EventMeshDBContext.SemaphoreSlim.Release();
            return result;
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
