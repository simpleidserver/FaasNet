using Coeus;
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

            var result = JQ.EvalToToken(filter, token);
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
    }
}