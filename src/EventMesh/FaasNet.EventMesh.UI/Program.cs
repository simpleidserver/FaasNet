using FaasNet.EventMesh.UI;
using FaasNet.EventMesh.UI.Data;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.Configure<EventMeshUIOptions>((o) => { });
builder.Services.AddTransient<IEventMeshService, EventMeshService>();
builder.Services.AddTransient<IPeerClientFactory, PeerClientFactory>();
builder.Services.AddTransient<IClientTransportFactory, ClientTransportFactory>();
builder.Services.AddTransient<IClientTransport, ClientUDPTransport>();

await builder.Build().RunAsync();