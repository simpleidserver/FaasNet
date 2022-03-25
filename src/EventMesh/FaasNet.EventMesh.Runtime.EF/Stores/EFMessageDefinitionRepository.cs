using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.EF.Stores
{
    public class EFMessageDefinitionRepository : IMessageDefinitionRepository
    {
        public void Add(MessageDefinition messageDefinition)
        {
            throw new NotImplementedException();
        }

        public Task<MessageDefinition> Get(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MessageDefinition>> GetLatestMessages(string applicationDomainId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Update(MessageDefinition messageDefinition)
        {
            throw new NotImplementedException();
        }
    }
}
