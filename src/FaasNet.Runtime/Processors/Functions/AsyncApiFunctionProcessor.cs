using FaasNet.Runtime.AsyncAPI;
using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;
using FaasNet.Runtime.Exceptions;
using FaasNet.Runtime.Resources;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors.Functions
{
    public class AsyncApiFunctionProcessor : IFunctionProcessor
    {
        private readonly IAsyncAPIParser _asyncAPIParser;

        public AsyncApiFunctionProcessor(IAsyncAPIParser asyncAPIParser)
        {
            _asyncAPIParser = asyncAPIParser;
        }

        public WorkflowDefinitionTypes Type => WorkflowDefinitionTypes.ASYNCAPI;

        public async Task<JToken> Process(JToken input, WorkflowDefinitionFunction function, WorkflowInstanceState instanceState, CancellationToken cancellationToken)
        {
            AsyncApiResult apiResult;
            if (!_asyncAPIParser.TryParseUrl(function.Operation, out apiResult))
            {
                throw new ProcessorException(string.Format(Global.InvalidAsyncApiUrl, function.Operation));
            }

            await _asyncAPIParser.Invoke(apiResult.Url, apiResult.OperationId, input, instanceState.Parameters, cancellationToken);
            return JToken.Parse("{}");
        }
    }
}
