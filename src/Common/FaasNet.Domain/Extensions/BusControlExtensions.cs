using FaasNet.Domain;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Domain.Extensions
{
    public static class BusControlExtensions
    {
        public static async Task PublishAll<T>(this IBusControl busControl, IEnumerable<T> evts, CancellationToken cancellationToken) where T : IntegrationEvent
        {
            foreach(var evt in evts)
            {
                var type = evt.GetType();
                await busControl.Publish(Convert.ChangeType(evt, type), cancellationToken);
            }
        }
    }
}
