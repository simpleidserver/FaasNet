using FaasNet.EventMesh.UI;
using FaasNet.EventMesh.UI.Data;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;
using Fluxor;
using Microsoft.Extensions.Options;
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


// https://github.com/mrpmorris/Fluxor/blob/f1bf7ce0df0d4bbcf8125b8dca8df6af603eda26/Source/Lib/Fluxor/DependencyInjection/ReflectionScanner.cs#L67

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