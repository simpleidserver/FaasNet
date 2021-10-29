using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Domains.Enums;
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
