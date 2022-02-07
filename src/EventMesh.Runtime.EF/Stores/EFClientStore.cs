using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;

namespace EventMesh.Runtime.EF.Stores
{
    public class EFClientStore : IClientStore
    {
        private readonly EventMeshDBContext _dbContext;

        public EFClientStore(EventMeshDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Client session)
        {
            _dbContext.Clients.Add(session);
            _dbContext.SaveChanges();
        }

        public Client Get(string clientId)
        {
            return _dbContext.Clients
                .Include(c => c.Sessions).ThenInclude(c => c.Histories)
                .Include(c => c.Sessions).ThenInclude(c => c.Topics)
                .Include(c => c.Sessions).ThenInclude(c => c.PendingCloudEvents)
                .Include(c => c.Topics)
                .FirstOrDefault(c => c.ClientId == clientId);
        }

        public Client GetByActiveSession(IPEndPoint edp)
        {
            var payload = edp.Address.GetAddressBytes();
            return _dbContext.Clients
                .Include(c => c.Sessions).ThenInclude(c => c.Histories)
                .Include(c => c.Sessions).ThenInclude(c => c.Topics)
                .Include(c => c.Sessions).ThenInclude(c => c.PendingCloudEvents)
                .Include(c => c.Topics)
                .FirstOrDefault(c => c.Sessions.Any(s => s.IPAddressData.SequenceEqual(payload) && s.Port == edp.Port));
        }

        public void Update(Client client)
        {
            _dbContext.Clients.Update(client);
            _dbContext.SaveChanges();
        }
    }
}
