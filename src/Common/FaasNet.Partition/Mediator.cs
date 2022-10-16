using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public interface IMediator
    {
        Task Send<T>(T cmd, CancellationToken cancellationToken) where T : class;
    }

    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Send<T>(T cmd, CancellationToken cancellationToken) where T : class
        {
            var consumerType = typeof(IConsumer<>).MakeGenericType(typeof(T));
            var enumerableConsumerType = typeof(IEnumerable<>).MakeGenericType(consumerType);
            var consumers = (IEnumerable<IConsumer<T>>)_serviceProvider.GetRequiredService(enumerableConsumerType);
            foreach (var consumer in consumers) await consumer.Consume(cmd, cancellationToken);
        }
    }
}
