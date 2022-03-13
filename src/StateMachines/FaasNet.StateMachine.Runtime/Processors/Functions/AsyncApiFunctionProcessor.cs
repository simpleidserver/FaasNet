using FaasNet.StateMachine.Runtime.AsyncAPI;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Exceptions;
using FaasNet.StateMachine.Runtime.Resources;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors.Functions
{
    public class AsyncApiFunctionProcessor : IFunctionProcessor
    {
        private readonly IAsyncAPIParser _asyncAPIParser;

        public AsyncApiFunctionProcessor(IAsyncAPIParser asyncAPIParser)
        {
            _asyncAPIParser = asyncAPIParser;
        }

        public StateMachineDefinitionTypes Type => StateMachineDefinitionTypes.ASYNCAPI;

        public async Task<JToken> Process(JToken input, StateMachineDefinitionFunction function, StateMachineInstanceState instanceState, Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            AsyncApiResult apiResult;
            if (!_asyncAPIParser.TryParseUrl(function.Operation, out apiResult))
            {
                throw new ProcessorException(string.Format(Global.InvalidAsyncApiUrl, function.Operation));
            }

            await _asyncAPIParser.Invoke(apiResult.Url, apiResult.OperationId, input, parameters, cancellationToken);
            return JToken.Parse("{}");
        }
    }
}
