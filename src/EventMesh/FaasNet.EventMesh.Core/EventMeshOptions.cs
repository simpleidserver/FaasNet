namespace FaasNet.EventMesh.Core
{
    public class EventMeshOptions
    {
        public EventMeshOptions()
        {
            Urn = "localhost";
            Port = 4000;
            StateMachineClientId = "stateMachineClientId";
        }

        public string Urn { get; set; }
        public int Port { get; set; }
        public string StateMachineClientId { get; set; }
    }
}
