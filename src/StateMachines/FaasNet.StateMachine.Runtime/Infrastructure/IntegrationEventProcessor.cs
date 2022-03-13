using FaasNet.StateMachine.Runtime.Domains.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Infrastructure
{
    public class IntegrationEventProcessor : IIntegrationEventProcessor
    {
        private readonly IServiceProvider _serviceProvider;

        public IntegrationEventProcessor(IServiceProvider serviceProviders)
        {
            _serviceProvider = serviceProviders;
        }

        public async Task Process(List<IntegrationEvent> integrationEvts, CancellationToken cancellationToken)
        {
            var types = integrationEvts.Select(i => i.GetType()).Distinct();
            foreach(var type in types)
            {
                var genericType = typeof(IIntegrationEventHandler<>).MakeGenericType(type);
                var service = _serviceProvider.GetService(genericType);
                var processMethod = genericType.GetMethod("Process", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var filteredEvts = integrationEvts.Where(e => e.GetType() == type);
                foreach(var evt in filteredEvts)
                {
                    var t = (Task)processMethod.Invoke(service, new object[] { evt, cancellationToken });
                    await t;
                }
            }
        }
    }
}
