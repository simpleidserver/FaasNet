using FaasNet.DHT.Chord.Core;
using FaasNet.DHT.Chord.Core.Handlers;
using FaasNet.DHT.Chord.Core.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder AddDHTChord(this ServerBuilder serverBuilder)
        {
            serverBuilder.Services.AddLogging();
            serverBuilder.Services.AddTransient<IDHTPeerFactory, DHTPeerFactory>();
            serverBuilder.Services.AddTransient<IDHTPeer, DHTPeer>();
            serverBuilder.Services.AddTransient<IRequestHandler, GetDimensionFingerTableRequestHandler>();
            serverBuilder.Services.AddTransient<IRequestHandler, JoinChordNetworkRequestHandler>();
            serverBuilder.Services.AddTransient<IRequestHandler, FindSuccessorRequestHandler>();
            serverBuilder.Services.AddScoped<IDHTPeerInfoStore, DHTPeerInfoStore>();
            return serverBuilder;
        }
    }
}
