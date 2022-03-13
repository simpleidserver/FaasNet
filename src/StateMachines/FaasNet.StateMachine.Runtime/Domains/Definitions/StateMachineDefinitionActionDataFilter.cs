
namespace FaasNet.StateMachine.Runtime.Domains.Definitions
{
    public class StateMachineDefinitionActionDataFilter
    {
        /// <summary>
        /// Workflow expression that filters state data that can be used by the action.
        /// </summary>
        public string FromStateData { get; set; }
        /// <summary>
        /// If set to false, action data results are not added/merged to state data. In this case 'results' and 'toStateData' should be ignored. Default is true.
        /// </summary>
        public bool UseResults { get; set; }
        /// <summary>
        /// Workflow expression that filters the actions data results
        /// </summary>
        public string Results { get; set; }
        /// <summary>
        /// Workflow expression that selects a state data element to which the action results should be added/merged into. If not specified denotes the top-level state data element.
        /// </summary>
        public string ToStateData { get; set; }
    }
}
