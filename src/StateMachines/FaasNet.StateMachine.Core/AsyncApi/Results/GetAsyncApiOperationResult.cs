using FaasNet.StateMachine.Runtime.AsyncAPI.v2.Models;

namespace FaasNet.StateMachine.Core.OpenApi.Results
{
    public class GetAsyncApiOperationResult
    {
        public Operation AsyncApiOperation { get; set; }
        public Components Components { get; set; }
    }
}
