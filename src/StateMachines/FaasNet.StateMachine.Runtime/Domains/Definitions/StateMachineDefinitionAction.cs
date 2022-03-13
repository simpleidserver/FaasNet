
namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionAction
    {
        /// <summary>
        /// Unique action name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// References a resusable function definition.
        /// </summary>
        public virtual StateMachineDefinitionFunctionRef FunctionRef { get; set; }
        /// <summary>
        /// Action data filter definition.
        /// </summary>
        public virtual StateMachineDefinitionActionDataFilter ActionDataFilter { get; set; }
        public virtual StateMachineDefinitionCallbackState CallbackState { get; set; }
    }
}
