using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.EF.Stores
{
    public class EFMessageDefinitionRepository : IMessageDefinitionRepository
    {
        private readonly EventMeshDBContext _dbContext;

        public EFMessageDefinitionRepository(EventMeshDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(MessageDefinition messageDefinition)
        {
            _dbContext.MessageDefinitionLst.Add(messageDefinition);
        }

        public Task<MessageDefinition> Get(string id, CancellationToken cancellationToken)
        {
            return _dbContext.MessageDefinitionLst.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<MessageDefinition>> GetLatestMessages(string applicationDomainId, CancellationToken cancellationToken)
        {
            await EventMeshDBContext.SemaphoreSlim.WaitAsync();
            var messages = await _dbContext.MessageDefinitionLst.OrderByDescending(m => m.Version).Where(m => m.ApplicationDomainId == applicationDomainId).ToListAsync(cancellationToken);
            EventMeshDBContext.SemaphoreSlim.Release();
            return messages.GroupBy(m => m.Name).Select(m => m.First()).OrderByDescending(m => m.UpdateDateTime);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Update(MessageDefinition messageDefinition)
        {
            _dbContext.MessageDefinitionLst.Update(messageDefinition);
        }
    }
}
