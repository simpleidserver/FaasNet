namespace FaasNet.StateMachine.Worker.EventMesh
{
    public class EventMeshOptions
    {
        public EventMeshOptions()
        {
            ClientId = "stateMachineClientId";
            Password = "password";
            Url = "localhost";
            Port = 4889;
        }

        public string ClientId {  get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }
}
