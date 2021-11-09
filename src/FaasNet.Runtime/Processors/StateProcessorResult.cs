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
        public bool IsEnd { get; private set; }
        public string Transition { get; private set; }
        public StateProcessorStatus Status { get; set; }

        public static StateProcessorResult End(JObject result)
        {
            return new StateProcessorResult(result)
            {
                Status = StateProcessorStatus.OK,
                IsEnd = true
            };
        }

        public static StateProcessorResult Next(JObject result, string transition)
        {
            return new StateProcessorResult(result)
            {
                Status = StateProcessorStatus.OK,
                Transition = transition
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
