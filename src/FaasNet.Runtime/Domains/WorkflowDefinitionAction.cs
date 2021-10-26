
namespace FaasNet.Runtime.Domains
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
    }
}
