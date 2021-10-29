using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaasNet.Runtime.Extensions
{
    public static class JObjectExtensions
    {
        public static JObject Transform(this JObject jObj, string filter)
        {
            if(string.IsNullOrWhiteSpace(filter))
            {
                return jObj;
            }

            var regularExpression = new Regex(@"\$\.([a-zA-Z])*");
            var filterObj = JObject.Parse(filter);
            var transformed = new JObject();
            foreach (var record in filterObj)
            {
                var val = record.Value.ToString();
                if (!val.StartsWith("$"))
                {
                    val = regularExpression.Replace(val, (m) =>
                    {
                        var token = jObj.SelectToken(m.Value);
                        if (token == null)
                        {
                            return m.Value;
                        }

                        return token.ToString();
                    });
                    transformed.Add(record.Key, val);
                    continue;
                }

                var tokens = jObj.SelectTokens(val);
                if (!tokens.Any())
                {
                    continue;
                }

                if (tokens.Count() == 1)
                {
                    transformed.Add(record.Key, tokens.First());
                }
                else
                {
                    transformed.Add(record.Key, new JArray(tokens));
                }
            }

            return transformed;
        }

        public static void Merge(this JObject jObj, string targetPath, JToken token)
        {
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                jObj.Merge(token, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                return;
            }

            var selectedToken = jObj.SelectToken(targetPath);
            if (selectedToken != null)
            {
                Merge(selectedToken, token);
            }
            else
            {
                // TODO : Correctly tokenize the JSONExpression.
                var names = targetPath.Split('.').Skip(1);
                JToken parentNode = jObj;
                for (var i = 0; i < names.Count(); i++)
                {
                    var name = names.ElementAt(i);
                    selectedToken = parentNode.SelectToken(name);
                    if (selectedToken != null)
                    {
                        parentNode = selectedToken;
                    }
                    else
                    {
                        JToken record = new JObject();
                        if (i == names.Count() - 1)
                        {
                            record = token;
                        }

                        (parentNode as JObject).Add(name, record);
                        parentNode = record;
                    }
                }
            }
        }

        private static void Merge(JToken selectedToken, JToken token)
        {
            var jVal = selectedToken as JValue;
            var jObject = selectedToken as JObject;
            var jArr = selectedToken as JArray;
            if (jVal != null)
            {
                var property = jVal.Parent as JProperty;
                property.Value = token;
            }
            else if (jObject != null)
            {
                jObject.Remove();
                var parent = jObject.Parent;
                parent.Add(token);
            }
            else if (jArr != null)
            {
                jArr.Add(token);
            }
        }
    }
}
