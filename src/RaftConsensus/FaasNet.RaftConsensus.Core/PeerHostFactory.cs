using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.RaftConsensus.Core
{
    public interface IPeerHostFactory
    {
        IPeerHost Build();
    }

    public class PeerHostFactory : IPeerHostFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PeerHostFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPeerHost Build()
        {
            var scope = _serviceProvider.CreateScope();
            return (IPeerHost)scope.ServiceProvider.GetService(typeof(IPeerHost));
        }
    }
}
