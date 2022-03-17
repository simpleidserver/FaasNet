﻿using FaasNet.StateMachine.Runtime.Domains.Subscriptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.Persistence
{
    public interface ICloudEventSubscriptionRepository
    {
        IQueryable<CloudEventSubscriptionAggregate> Query();
        Task Add(CloudEventSubscriptionAggregate evt, CancellationToken cancellationToken);
        Task Update(IEnumerable<CloudEventSubscriptionAggregate> evt, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}