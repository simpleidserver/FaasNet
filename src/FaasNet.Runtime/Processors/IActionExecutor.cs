using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Processors
{
    public interface IActionExecutor
    {
        Task<JObject> Execute(JObject inputState, JObject input, WorkflowDefinitionActionModes actionMode, ICollection<WorkflowDefinitionAction> actions, WorkflowInstanceExecutionContext executionContext, CancellationToken cancellationToken);
    }
}
