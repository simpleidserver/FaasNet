using FaasNet.Gateway.Core.Domains;

namespace FaasNet.Gateway.Core.EventMeshServer.Queries.Results
{
    public class EventMeshServerBridgeResult
    {
        public string Urn { get; set; }
        public int Port { get; set; }

        public static EventMeshServerBridgeResult ToDto(EventMeshServerBridge parameter)
        {
            return new EventMeshServerBridgeResult
            {
                Urn = parameter.Urn,
                Port = parameter.Port
            };
        }
    }
}
