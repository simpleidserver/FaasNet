using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EventMesh.Runtime.EF.Stores
{
    public class EFClientStore : IClientStore
    {
        private static object _obj = new object();
        private readonly EventMeshDBContext _dbContext;

        public EFClientStore(EventMeshDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Client session)
        {
            lock (_obj)
            {
                _dbContext.Clients.Add(session);
                _dbContext.SaveChanges();
            }
        }

        public int Count()
        {
            lock(_obj)
                return _dbContext.Clients.Count();
        }

        public int CountActiveSessions()
        {
            lock(_obj)
                return _dbContext.Clients.SelectMany(c => c.Sessions).Count(c => c.State == ClientSessionState.ACTIVE);
        }

        public Client Get(string clientId)
        {
            lock(_obj)
                return _dbContext.Clients
                    .Include(c => c.Sessions).ThenInclude(c => c.Histories)
                    .Include(c => c.Sessions).ThenInclude(c => c.Topics)
                    .Include(c => c.Sessions).ThenInclude(c => c.PendingCloudEvents)
                    .Include(c => c.Sessions).ThenInclude(c => c.Bridges)
                    .Include(c => c.Topics)
                    .FirstOrDefault(c => c.ClientId == clientId);
        }

        public IEnumerable<Client> GetAll()
        {
            lock(_obj)
                return _dbContext.Clients
                    .Include(c => c.Sessions)
                    .Include(c => c.Topics)
                    .ToList();
        }

        public Client GetByActiveSession(string clientId, string sessionId)
        {
            lock(_obj)
                return _dbContext.Clients
                    .Include(c => c.Sessions).ThenInclude(c => c.Histories)
                    .Include(c => c.Sessions).ThenInclude(c => c.Topics)
                    .Include(c => c.Sessions).ThenInclude(c => c.PendingCloudEvents)
                    .Include(c => c.Sessions).ThenInclude(c => c.Bridges)
                    .Include(c => c.Topics)
                    .FirstOrDefault(c => c.ClientId == clientId && c.Sessions.Any(s => s.Id == sessionId));
        }

        public void Update(Client client)
        {
            lock(_obj)
            {
                _dbContext.Clients.Update(client);
                _dbContext.SaveChanges();
            }
        }
    }
}
