using Coeus;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace FaasNet.Runtime.Extensions
{
    public static class JTokenExtensions
    {
        private const string REGEX_STR = "\"?\\$\\{(\"| |\\w|\\.|>|<|=|\\[|\\]|\\)|\\(|\\|)*\\}\"?";

        public static JToken Transform(this JToken token, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return token;
            }

            var regex = new Regex(REGEX_STR);
            var str  = regex.Replace(filter, (m) =>
            {
                var value = Clean(m.Value);
                var r = JQ.EvalToToken(value, token);
                var str = r.ToString().Trim('#');
                if (r.Type == JTokenType.String)
                {
                    return $"\"{str}\"";
                }

                return str;
            });
            JToken result = null;
            try
            {
                result = JToken.Parse(str);
            }
            catch
            {
                result = str;
            }

            return result;
        }

        public static void Merge(this JToken jObj, JToken dataObj, string data, string toState)
        {
            JToken record = dataObj;
            if (!string.IsNullOrWhiteSpace(data))
            {
                record = record.Transform(data);
                if (record == null)
                {
                    return;
                }
            }

            jObj.Merge(toState, record);
        }

        public static void Merge(this JToken jObj, string targetPath, JToken token)
        {
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                Merge(jObj, token);
                return;
            }

            var regex = new Regex(REGEX_STR);
            targetPath = regex.Replace(targetPath, (m) =>
            {
                return Clean(m.Value);
            });
            var tok = JQ.EvalToToken(targetPath, jObj);
            if (tok.Type == JTokenType.Null)
            {
                var emptyObj = JQ.ParseToEmptyObject(targetPath);
                emptyObj.JObj.SelectToken(emptyObj.FullPath).Replace(token);
                Merge(jObj, emptyObj.JObj);
                return;
            }

            tok.Replace(token);
        }

        public static string CleanExpression(string filter)
        {
            var regex = new Regex(REGEX_STR);
            return regex.Replace(filter, (m) =>
            {
                return Clean(m.Value);
            });
        }

        private static void Merge(JToken jObj, JToken token)
        {
            if (jObj.Type == JTokenType.Array)
            {
                var jArr = jObj as JArray;
                jArr.Add(token);
            }
            else if (jObj.Type == JTokenType.Object)
            {
                var j = jObj as JObject;
                j.Merge(token, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }
        }

        private static string Clean(string str)
        {
            return str.Trim('"')
                       .Trim('#')
                       .TrimStart('$').TrimStart('{')
                       .TrimEnd('}')
                       .Trim();
        }
    }
}