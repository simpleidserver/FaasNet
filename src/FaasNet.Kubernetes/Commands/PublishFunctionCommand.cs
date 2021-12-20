namespace FaasNet.Kubernetes.Commands
{
    public class PublishFunctionCommand
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string Version { get; set; }
    }
}
