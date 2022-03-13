namespace FaasNet.CLI.Parameters
{
    public class InvokeFunctionParameter
    {
        [Parameter("-id", true)]
        public string Id { get; set; }
        [Parameter("-configuration", false, "{}")]
        public string Configuration { get; set; }
        [Parameter("-input", false, "{}")]
        public string Input { get; set; }
    }
}
