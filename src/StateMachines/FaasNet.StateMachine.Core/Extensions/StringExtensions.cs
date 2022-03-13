using Newtonsoft.Json.Linq;

namespace FaasNet.StateMachine.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsJson(this string str)
        {
            try
            {
                JObject.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
