namespace FaasNet.Runtime.OpenAPI.Models
{
    public class OpenApiParameterResult
    {
        public string Name { get; set; }
        public string In { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public OpenApiSchemaPropertyResult Schema { get; set; }
    }
}
