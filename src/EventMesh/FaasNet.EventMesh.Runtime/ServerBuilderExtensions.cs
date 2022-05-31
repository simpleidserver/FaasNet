using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder AddVpns(this ServerBuilder serverBuilder, IEnumerable<Vpn> vpns)
        {
            var serviceProvider = serverBuilder.ServiceProvider;
            var vpnStore = serviceProvider.GetRequiredService<IVpnStore>();
            foreach (var vpn in vpns) vpnStore.Add(vpn, CancellationToken.None);
            return serverBuilder;
        }

        public static ServerBuilder AddClients(this ServerBuilder serverBuilder, IEnumerable<Client> clients)
        {
            var serviceProvider = serverBuilder.ServiceProvider;
            var clientStore = serviceProvider.GetRequiredService<IClientStore>();
            foreach (var client in clients) clientStore.Add(client, CancellationToken.None);
            return serverBuilder;
        }
    }
}
