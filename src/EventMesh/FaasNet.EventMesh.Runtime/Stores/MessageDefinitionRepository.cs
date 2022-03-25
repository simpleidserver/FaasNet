using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public class MessageDefinitionRepository : IMessageDefinitionRepository
    {
        private readonly ICollection<MessageDefinition> _messageDefinitions;

        public MessageDefinitionRepository(ICollection<MessageDefinition> messageDefinitions)
        {
            _messageDefinitions = messageDefinitions;
        }

        public void Add(MessageDefinition messageDefinition)
        {
            _messageDefinitions.Add(messageDefinition);
        }

        public Task<MessageDefinition> Get(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_messageDefinitions.FirstOrDefault(md => md.Id == id));
        }

        public Task<IEnumerable<MessageDefinition>> GetLatestMessages(string applicationDomainId, CancellationToken cancellationToken)
        {
            var result = _messageDefinitions.OrderByDescending(m => m.Version).GroupBy(m => m.Name).Select(m => m.First());
            return Task.FromResult(result);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public void Update(MessageDefinition messageDefinition)
        {
            _messageDefinitions.Remove(messageDefinition);
            _messageDefinitions.Add(messageDefinition);
        }
    }
}
