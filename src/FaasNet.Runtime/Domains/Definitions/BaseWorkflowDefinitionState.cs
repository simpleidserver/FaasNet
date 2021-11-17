using FaasNet.Runtime.Domains.Enums;
using YamlDotNet.Serialization;

namespace FaasNet.Runtime.Domains.Definitions
{
    public abstract class BaseWorkflowDefinitionState
    {
        /// <summary>
        /// Unique state id.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// State name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// State type.
        /// </summary>
        public WorkflowDefinitionStateTypes Type { get; set; }
        /*
        /// <summary>
        /// End date.
        /// </summary>
        public bool End { get; set; }
        /// <summary>
        /// Next transition of the workflow after all the actions have been performed.
        /// </summary>
        public string Transition { get; set; }
        */
        /// <summary>
        /// Workflow expression to filter the states data input.
        /// </summary>
        public string StateDataFilterInput { get; set; }
        /// <summary>
        /// Workflow expression to filter the states data output.
        /// </summary>
        public string StateDataFilterOuput { get; set; }
        [YamlIgnore]
        public bool IsRootState { get; set; }
    }
}
