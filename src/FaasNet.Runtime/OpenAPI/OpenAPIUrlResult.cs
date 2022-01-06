namespace FaasNet.Runtime.OpenAPI
{
    public record OpenAPIUrlResult
    {
        public OpenAPIUrlResult(string url, string operationId)
        {
            Url = url;
            OperationId = operationId;
        }

        public string Url { get; set; }
        public string OperationId { get; set; }
    }
}
