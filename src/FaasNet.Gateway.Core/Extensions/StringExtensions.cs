using Newtonsoft.Json.Linq;

namespace FaasNet.Gateway.Core.Extensions
{
    public static class StringExtensions
    {
        public static string CleanPath(this string str)
        {
            return str.TrimStart('/');
        }

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
