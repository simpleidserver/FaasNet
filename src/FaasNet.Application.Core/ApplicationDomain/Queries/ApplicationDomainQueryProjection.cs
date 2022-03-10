using FaasNet.Application.Core.Domains.DomainEvents;
using FaasNet.EventStore;
using System.Threading.Tasks;

namespace FaasNet.Application.Core.ApplicationDomain.Queries
{
    public class ApplicationDomainQueryProjection : BaseQueryProjection
    {
        public ApplicationDomainQueryProjection(IEventStoreConsumer eventStoreConsumer) : base(eventStoreConsumer)
        {

        }

        protected override string Topic => throw new System.NotImplementedException();
        protected override string GroupId => throw new System.NotImplementedException();

        protected override void Project(ProjectionBuilder builder)
        {
            builder.On((ApplicationDomainCreatedEvent evt) =>
            {
                return Task.CompletedTask;
            });
        }
    }
}
