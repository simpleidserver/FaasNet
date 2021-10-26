using Newtonsoft.Json.Linq;

namespace FaasNet.Runtime.Processors
{
    public class StateProcessorResult
    {
        private StateProcessorResult(string output)
        {
            Output = output;
            IsError = false;
        }

        public string Output { get; private set; }
        public bool IsError { get; private set; }

        public static StateProcessorResult Ok(string result)
        {
            return new StateProcessorResult(result);
        }
    }
}
