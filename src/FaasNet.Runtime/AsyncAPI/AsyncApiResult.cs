namespace FaasNet.Runtime.AsyncAPI
{
    public class AsyncApiResult
    {
        public AsyncApiResult(string url, string operationId)
        {
            Url = url;
            OperationId = operationId;
        }

        public string Url { get; }
        public string OperationId { get; }
    }
}
