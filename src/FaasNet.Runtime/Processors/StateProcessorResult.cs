using Newtonsoft.Json.Linq;
using System;

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

        private StateProcessorResult(JToken output)
        {
            Output = output;
        }

        public JToken Output { get; private set; }
        public string Exception { get; private set; }
        public bool IsEnd { get; private set; }
        public string Transition { get; private set; }
        public StateProcessorStatus Status { get; set; }

        public static StateProcessorResult End(JToken result)
        {
            return new StateProcessorResult(result)
            {
                Status = StateProcessorStatus.OK,
                IsEnd = true
            };
        }

        public static StateProcessorResult Next(JToken result, string transition)
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

        public static StateProcessorResult Error(Exception exception)
        {
            return new StateProcessorResult
            {
                Status = StateProcessorStatus.ERROR,
                Exception = exception.ToString()
            };
        }
    }
}
