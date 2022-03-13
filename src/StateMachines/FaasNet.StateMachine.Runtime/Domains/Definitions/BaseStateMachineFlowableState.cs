
namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class BaseStateMachineFlowableState : BaseStateMachineDefinitionState
    {
        /// <summary>
        /// End date.
        /// </summary>
        public bool End { get; set; }
        /// <summary>
        /// Next transition of the workflow after all the actions have been performed.
        /// </summary>
        public string Transition { get; set; }
    }
}
