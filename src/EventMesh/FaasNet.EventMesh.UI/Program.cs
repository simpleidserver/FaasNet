using FaasNet.EventMesh.UI;
using FaasNet.EventMesh.UI.Data;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;
using Fluxor;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.Configure<EventMeshUIOptions>((o) => { });
builder.Services.AddScoped<ToastService, ToastService>();
builder.Services.AddSingleton<IEventMeshService, EventMeshService>();
builder.Services.AddTransient<IPeerClientFactory, PeerClientFactory>();
builder.Services.AddTransient<IClientTransportFactory, ClientTransportFactory>();
builder.Services.AddTransient<IClientTransport, ClientUDPTransport>();

builder.Services.AddFluxor(o =>
{
    o.ScanAssemblies(Assembly.GetExecutingAssembly());
});
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();