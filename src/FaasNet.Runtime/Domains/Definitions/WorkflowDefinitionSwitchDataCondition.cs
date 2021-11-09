
namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionSwitchDataCondition : BaseEventCondition
    {
        public WorkflowDefinitionSwitchDataCondition()
        {
            ConditionType = Enums.WorkflowDefinitionEventConditionTypes.DATA;
        }

        /// <summary>
        /// Workflow expression evaluated against state data. Must evaluate to true or false.
        /// </summary>
        public string Condition { get; set; }
    }
}
