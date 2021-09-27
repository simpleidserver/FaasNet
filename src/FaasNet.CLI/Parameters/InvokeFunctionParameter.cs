namespace FaasNet.CLI.Parameters
{
    public class InvokeFunctionParameter
    {
        [Parameter("-name", true)]
        public string Name { get; set; }
        [Parameter("-configuration", false, "{}")]
        public string Configuration { get; set; }
        [Parameter("-input", false, "{}")]
        public string Input { get; set; }
    }
}
