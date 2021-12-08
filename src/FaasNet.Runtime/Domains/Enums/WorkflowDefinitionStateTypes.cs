using System.Runtime.Serialization;

namespace FaasNet.Runtime.Domains.Enums
{
    public enum WorkflowDefinitionStateTypes
    {
        /// <summary>
        /// Define events that trigger action execution.
        /// </summary>
        [EnumMember(Value = "event")]
        Event = 0,
        /// <summary>
        /// Execute one or more actions.
        /// </summary>
        [EnumMember(Value = "operation")]
        Operation = 1,
        /// <summary>
        /// Define data-based or event-based workflow transitions.
        /// </summary>
        [EnumMember(Value = "switch")]
        Switch = 2,
        /// <summary>
        /// Sleep workflow execution for a specific time duration.
        /// </summary>
        [EnumMember(Value = "sleep")]
        Sleep = 3,
        /// <summary>
        /// Causes parallel execution of branches (set of states).
        /// </summary>
        [EnumMember(Value = "parallel")]
        Parallel = 4,
        /// <summary>
        /// Inject-static data into state data.
        /// </summary>
        [EnumMember(Value = "inject")]
        Inject = 5,
        /// <summary>
        /// Parallel execution of states for each element of a data array.
        /// </summary>
        [EnumMember(Value = "foreach")]
        ForEach = 6,
        /// <summary>
        /// Manual decision step. Executes a function and waits for callback event that indicates completion of the manual decision.
        /// </summary>
        [EnumMember(Value = "callback")]
        Callback = 7
    }
}
