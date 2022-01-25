using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors
{
    public interface IFunctionProcessor
    {
        WorkflowDefinitionTypes Type { get; }
        Task<JToken> Process(JToken input, WorkflowDefinitionFunction function, WorkflowInstanceState instanceState, Dictionary<string, string> parameters, CancellationToken cancellationToken);
    }
}
