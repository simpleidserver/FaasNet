using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Domain
{
    public abstract class AggregateRoot
    {
        public AggregateRoot()
        {
            DomainEvts = new List<DomainEvent>();
            IntegrationEvents = new List<IntegrationEvent>();
        }

        #region Properties

        public string Id { get; set; }
        public int Version { get; set; }
        public int LastEvtOffset { get; set; }
        public ICollection<DomainEvent> DomainEvts { get; set; }
        public ICollection<IntegrationEvent> IntegrationEvents { get; set; }

        #endregion

        public abstract string Topic { get; }

        public abstract void Handle(dynamic evt);

        public void Commit()
        {
            Version++;
            foreach(var domainEvt in DomainEvts)
            {
                domainEvt.AggregateVersion = Version;
            }

            LastEvtOffset += DomainEvts.Count();
        }
    }
}
