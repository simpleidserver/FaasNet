using FaasNet.Runtime.AsyncAPI;
using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;
using Newtonsoft.Json.Linq;
using System.Linq;
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
            var splitted = function.Operation.Split('#');
            await _asyncAPIParser.Invoke(splitted.First(), splitted.Last(), input, cancellationToken);
            return null;
        }
    }
}
