using FaasNet.StateMachine.Runtime.JSchemas;

namespace FaasNet.StateMachine.Runtime.OpenAPI.v3.Models
{
    public class OpenApiParameterResult
    {
        public string Name { get; set; }
        public string In { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public FaasNetJsonSchema Schema { get; set; }
    }
}
