using MediatR;

namespace FaasNet.Gateway.Core.EventMeshServer.Commands
{
    public class AddEventMeshServerBridgeCommand : IRequest<bool>
    {
        public EventMeshServer From { get; set; }
        public EventMeshServer To { get; set; }
    }

    public class EventMeshServer
    {
        public string Urn { get; set; }
        public int Port { get; set; }
    }
}
