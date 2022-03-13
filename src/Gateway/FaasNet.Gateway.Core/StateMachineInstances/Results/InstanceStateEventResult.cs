using FaasNet.StateMachine.Runtime.Domains.Instances;
using Newtonsoft.Json.Linq;

namespace FaasNet.Gateway.Core.StateMachineInstances.Results
{
    public class InstanceStateEventResult
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public JToken Input { get; set; }

        public static InstanceStateEventResult ToDto(WorkflowInstanceStateEvent evt)
        {
            return new InstanceStateEventResult
            {
                Input = evt.InputDataObj,
                Name = evt.Name,
                Source = evt.Source,
                Type = evt.Type
            };
        }
    }
}
