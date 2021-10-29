using Newtonsoft.Json.Linq;

namespace FaasNet.Runtime.Processors
{
    public class StateProcessorResult
    {
        private StateProcessorResult(JObject output)
        {
            Output = output;
            IsError = false;
        }

        public JObject Output { get; private set; }
        public bool IsError { get; private set; }

        public static StateProcessorResult Ok(JObject result)
        {
            return new StateProcessorResult(result);
        }
    }
}
