using FaasNet.Application.Core.ApplicationDomain.Queries.Results;
using FaasNet.Application.Core.Domains;
using FaasNet.Application.Core.Domains.DomainEvents;
using FaasNet.Application.Core.Repositories;
using FaasNet.EventStore;
using Microsoft.Extensions.Options;
using System.Threading;

namespace FaasNet.Application.Core.ApplicationDomain.Queries
{
    public class ApplicationDomainQueryProjection : BaseQueryProjection
    {
        private readonly IApplicationDomainQueryRepository _applicationDomainQueryRepository;
        private readonly ApplicationOptions _options;

        public ApplicationDomainQueryProjection(
            IEventStoreConsumer eventStoreConsumer,
            ISubscriptionRepository subscriptionRepository,
            IApplicationDomainQueryRepository applicationDomainQueryRepository,
            IOptions<ApplicationOptions> options) : base(eventStoreConsumer, subscriptionRepository)
        {
            _applicationDomainQueryRepository = applicationDomainQueryRepository;
            _options = options.Value;
        }

        protected override string Topic => ApplicationDomainAggregate.TOPIC_NAME;
        protected override string GroupId => _options.GroupId;

        protected override void Project(ProjectionBuilder builder)
        {
            builder.On(async (ApplicationDomainCreatedEvent evt) =>
            {
                var application = new ApplicationDomainResult
                {
                    Id = evt.AggregateId,
                    Description = evt.Description,
                    Name = evt.Name,
                    RootTopic = evt.RootTopic,
                    CreateDateTime = evt.CreateDateTime,
                    UpdateDateTime = evt.CreateDateTime
                };
                _applicationDomainQueryRepository.Add(application);
                await _applicationDomainQueryRepository.SaveChanges(CancellationToken.None);
            });
        }
    }
}
