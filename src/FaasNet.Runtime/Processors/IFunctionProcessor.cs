using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors
{
    public interface IFunctionProcessor
    {
        WorkflowDefinitionTypes Type { get; }
        Task<JObject> Process(JObject input, WorkflowDefinitionFunction function, WorkflowInstanceState instanceState, CancellationToken cancellationToken);
    }
}
