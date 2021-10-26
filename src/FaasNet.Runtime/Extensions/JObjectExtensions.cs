using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaasNet.Runtime.Extensions
{
    public static class JObjectExtensions
    {
        public static string Transform(this JObject jObj, string filter)
        {
            if(string.IsNullOrWhiteSpace(filter))
            {
                return jObj.ToString();
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

            return transformed.ToString();
        }
    }
}
