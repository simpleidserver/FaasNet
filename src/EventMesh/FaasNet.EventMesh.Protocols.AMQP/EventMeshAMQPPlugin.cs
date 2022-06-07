using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Protocols.AMQP
{
    public class EventMeshAMQPPlugin : IProtocolPlugin<EventMeshAMQPOptions>
    {
        public void Load(IServiceCollection services, EventMeshAMQPOptions options)
        {
            services.AddAMQPProtocol(o =>
            {
                o.Port = options.Port;
                o.EventMeshPort = options.EventMeshPort;
                o.EventMeshVpn = options.EventMeshVpn;
                o.EventMeshUrl = options.EventMeshUrl;
                o.MaxFrameSize = options.MaxFrameSize;
                o.MaxChannel = options.MaxChannel;
                o.SessionLinkCredit = options.SessionLinkCredit;
                o.SessionLinkCredit = options.SessionLinkCredit;
            });
        }
    }
}
