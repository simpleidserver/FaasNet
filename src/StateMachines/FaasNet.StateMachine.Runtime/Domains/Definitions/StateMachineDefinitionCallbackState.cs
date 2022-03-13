using FaasNet.StateMachine.Runtime.Domains.Enums;
using System;

namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionCallbackState : BaseStateMachineFlowableState
    {
        public StateMachineDefinitionCallbackState()
        {
            Type = StateMachineDefinitionStateTypes.Callback;
        }

        /// <summary>
        /// Defines the action to be executed.
        /// </summary>
        public virtual StateMachineDefinitionAction Action { get; set; }
        /// <summary>
        /// References an unique callback event name in the defined workflow events.
        /// </summary>
        public string EventRef { get; set; }
        public int? ActionId { get; set; }

        public static StateMachineDefinitionCallbackState Create()
        {
            return new StateMachineDefinitionCallbackState
            {
                Id = Guid.NewGuid().ToString(),
                Type = StateMachineDefinitionStateTypes.Callback
            };
        }
    }
}
