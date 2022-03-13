using FaasNet.Gateway.Core.Functions.Invokers;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Processors;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Processors
{
    public class CustomFunctionProcessor : IFunctionProcessor
    {
        private readonly IFunctionInvoker _functionInvoker;

        public CustomFunctionProcessor(IFunctionInvoker functionInvoker)
        {
            _functionInvoker = functionInvoker;
        }

        public WorkflowDefinitionTypes Type => WorkflowDefinitionTypes.CUSTOM;

        public Task<JToken> Process(JToken input, WorkflowDefinitionFunction function, WorkflowInstanceState instanceState,  Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            return _functionInvoker.Invoke(function.FunctionId, input, function.Configuration, cancellationToken);
        }
    }
}
