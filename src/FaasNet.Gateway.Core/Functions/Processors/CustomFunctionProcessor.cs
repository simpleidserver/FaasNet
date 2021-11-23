using FaasNet.Gateway.Core.Functions.Invokers;
using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;
using FaasNet.Runtime.Processors;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.Functions.Processors
{
    public class CustomFunctionProcessor : IFunctionProcessor
    {
        private readonly IFunctionInvokerFactory _functionInvokerFactory;

        public CustomFunctionProcessor(IFunctionInvokerFactory functionInvokerFactory)
        {
            _functionInvokerFactory = functionInvokerFactory;
        }

        public WorkflowDefinitionTypes Type => WorkflowDefinitionTypes.CUSTOM;

        public Task<JToken> Process(JToken input, WorkflowDefinitionFunction function, WorkflowInstanceState instanceState, CancellationToken cancellationToken)
        {
            var invoker = _functionInvokerFactory.Build(function.Provider);
            return invoker.Invoke(function.FunctionId, input, function.Configuration, cancellationToken);
        }
    }
}
