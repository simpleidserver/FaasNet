using Newtonsoft.Json.Linq;

namespace FaasNet.Runtime.Processors
{
    public enum StateProcessorStatus
    {
        OK = 0,
        ERROR = 1,
        BLOCKED = 2
    }

    public class StateProcessorResult
    {
        private StateProcessorResult() { }

        private StateProcessorResult(JObject output)
        {
            Output = output;
        }

        public JObject Output { get; private set; }
        public StateProcessorStatus Status { get; set; }

        public static StateProcessorResult Ok(JObject result)
        {
            return new StateProcessorResult(result)
            {
                Status = StateProcessorStatus.OK
            };
        }

        public static StateProcessorResult Block()
        {
            return new StateProcessorResult
            {
                Status = StateProcessorStatus.BLOCKED
            };
        }
    }
}
