using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Processors
{
    public interface IActionExecutor
    {
        Task<JToken> ExecuteAndMerge(JToken input, StateMachineDefinitionActionModes actionMode, ICollection<StateMachineDefinitionAction> actions, StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken);
        Task<JToken> ExecuteAndConcatenate(JToken input, StateMachineDefinitionActionModes actionMode, ICollection<StateMachineDefinitionAction> actions, StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken);
        Task<List<JToken>> Execute(JToken input, StateMachineDefinitionActionModes actionMode, ICollection<StateMachineDefinitionAction> actions, StateMachineInstanceExecutionContext executionContext, CancellationToken cancellationToken);
    }
}
