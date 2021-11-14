using Newtonsoft.Json.Linq;

namespace FaasNet.Runtime.Extensions
{
    public static class JTokenExtensions
    {
        public static JToken Transform(this JToken token, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return token;
            }

            var jObj = token as JObject;
            return jObj.Transform(filter);
        }

        public static void Merge(this JToken jObj, JObject dataObj, string data, string toState)
        {
            var obj = jObj as JObject;
            if (obj == null)
            {
                return;
            }

            obj.Merge(dataObj, data, toState);
        }
    }
}