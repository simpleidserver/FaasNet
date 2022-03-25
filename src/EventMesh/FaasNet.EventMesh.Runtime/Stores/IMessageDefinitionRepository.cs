using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IMessageDefinitionRepository
    {
        Task<MessageDefinition> Get(string id, CancellationToken cancellationToken);
        void Add(MessageDefinition messageDefinition);
        void Update(MessageDefinition messageDefinition);
        Task<int> SaveChanges(CancellationToken cancellationToken);
        Task<IEnumerable<MessageDefinition>> GetLatestMessages(string applicationDomainId, CancellationToken cancellationToken);
    }
}
