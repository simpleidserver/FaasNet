namespace FaasNet.Runtime.Domains
{
    public class WorkflowDefinitionEventDataFilter
    {
        public WorkflowDefinitionEventDataFilter()
        {
            UseData = true;
        }

        /// <summary>
        /// If set to false, event payload is not added/merged to state data. In this case 'data' and 'toStateData' should be ignored.
        /// Default is true.
        /// </summary>
        public bool UseData { get; set; }
        /// <summary>
        /// Workflow expression that filters the event data (payload).
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// Workflow expression that selects a state data element to which the action results should be added/merged into. 
        /// If not specified denotes the top-level state data element
        /// </summary>
        public string ToStateData { get; set; }
    }
}
