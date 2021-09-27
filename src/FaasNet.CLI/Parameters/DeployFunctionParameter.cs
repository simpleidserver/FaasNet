namespace FaasNet.CLI.Parameters
{
    public class DeployFunctionParameter
    {
        [Parameter("-name", true)]
        public string Name { get; set; }
        [Parameter("-image", true)]
        public string Image { get; set; }
    }
}
