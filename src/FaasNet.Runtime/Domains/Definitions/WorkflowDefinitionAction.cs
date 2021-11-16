
namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionAction
    {
        /// <summary>
        /// Unique action name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// References a resusable function definition.
        /// </summary>
        public WorkflowDefinitionFunctionRef FunctionRef { get; set; }
        /// <summary>
        /// Action data filter definition.
        /// </summary>
        public WorkflowDefinitionActionDataFilter ActionDataFilter { get; set; }
        public WorkflowDefinitionCallbackState CallbackState { get; set; }
    }
}
