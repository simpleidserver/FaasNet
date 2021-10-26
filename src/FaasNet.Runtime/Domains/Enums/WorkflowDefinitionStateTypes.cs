namespace FaasNet.Runtime.Domains.Enums
{
    public enum WorkflowDefinitionStateTypes
    {
        /// <summary>
        /// Define events that trigger action execution.
        /// </summary>
        Event = 0,
        /// <summary>
        /// Execute one or more actions.
        /// </summary>
        Operation = 1,
        /// <summary>
        /// Define data-based or event-based workflow transitions.
        /// </summary>
        Switch = 2,
        /// <summary>
        /// Sleep workflow execution for a specific time duration.
        /// </summary>
        Sleep = 3,
        /// <summary>
        /// Causes parallel execution of branches (set of states).
        /// </summary>
        Parallel = 4,
        /// <summary>
        /// Inject-static data into state data.
        /// </summary>
        Inject = 5,
        /// <summary>
        /// Parallel execution of states for each element of a data array.
        /// </summary>
        ForEach = 6,
        /// <summary>
        /// Manual decision step. Executes a function and waits for callback event that indicates completion of the manual decision.
        /// </summary>
        Callback = 7
    }
}
