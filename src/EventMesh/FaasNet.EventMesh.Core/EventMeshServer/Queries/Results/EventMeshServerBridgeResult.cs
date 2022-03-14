using FaasNet.EventMesh.Core.Domains;

namespace FaasNet.EventMesh.Core.EventMeshServer.Queries.Results
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
