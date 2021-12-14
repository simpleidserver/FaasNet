using FaasNet.Runtime.Domains.Enums;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace FaasNet.Runtime.Domains.Definitions
{
    public abstract class BaseWorkflowDefinitionState
    {
        [JsonIgnore]
        [YamlIgnore]
        /// <summary>
        /// Technical Identifier.
        /// </summary>
        public string TechnicalId { get; set; }
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
        [JsonIgnore]
        public string StateDataFilterInput { get; set; }
        /// <summary>
        /// Workflow expression to filter the states data output.
        /// </summary>
        [JsonIgnore]
        public string StateDataFilterOuput { get; set; }
        public StateDataFilter StateDataFilter
        {
            get
            {
                if (string.IsNullOrWhiteSpace(StateDataFilterOuput) && string.IsNullOrWhiteSpace(StateDataFilterOuput))
                {
                    return null;
                }

                return new StateDataFilter
                {
                    Input = StateDataFilterInput,
                    Output = StateDataFilterOuput
                };
            }
            set
            {
                StateDataFilterInput = value == null ? null : value.Input;
                StateDataFilterOuput = value == null ? null : value.Output;
            }
        }
    }

    public class StateDataFilter
    {
        public string Input { get; set; }
        public string Output { get; set; }
    }
}
