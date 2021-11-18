
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
        public virtual WorkflowDefinitionFunctionRef FunctionRef { get; set; }
        /// <summary>
        /// Action data filter definition.
        /// </summary>
        public virtual WorkflowDefinitionActionDataFilter ActionDataFilter { get; set; }
        public virtual WorkflowDefinitionCallbackState CallbackState { get; set; }
    }
}
