using FaasNet.EventMesh.UI;
using FaasNet.EventMesh.UI.Data;
using FaasNet.EventMesh.UI.ViewModels;
using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.Configure<EventMeshUIOptions>((o) => { });
builder.Services.AddScoped<EventMeshNodeViewModel, EventMeshNodeViewModel>();
builder.Services.AddSingleton<IEventMeshService, EventMeshService>();
builder.Services.AddTransient<IPeerClientFactory, PeerClientFactory>();
builder.Services.AddTransient<IClientTransportFactory, ClientTransportFactory>();
builder.Services.AddTransient<IClientTransport, ClientUDPTransport>();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();