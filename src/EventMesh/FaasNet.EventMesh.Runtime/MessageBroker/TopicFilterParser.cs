using FaasNet.EventMesh.Runtime.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public static class TopicFilterParser
    {
        private static string REGEX_PATTERN = $"^(\\w|[0-9]|\\.|\\*|\\-)*$";

        public static ICollection<TopicExpression> Parse(string topicFilter)
        {
            const string any = "*";
            var result = new List<TopicExpression>();
            if (topicFilter == any)
            {
                return result;
            }

            var regex = new Regex(REGEX_PATTERN);
            if (!regex.IsMatch(topicFilter))
            {
                throw new InvalidTopicFilterException(string.Format(Global.BadTopicFilterExpr, topicFilter));
            }

            var splitted = topicFilter.Split('.');
            if (splitted.Any(str => str.Length > 1 && str.Contains(any)))
            {
                throw new InvalidTopicFilterException(Global.BadAnyCharacterUsage);
            }

            var lst = new List<string>();
            int i = 0;
            foreach(var str in splitted)
            {
                if(str.Equals(any))
                {
                    var fullPath = string.Join(".", lst);
                    if (!result.Any())
                    {
                        result.Add(new TopicExpression(fullPath, TopicExpressionTypes.STARTWITH));
                    }
                    else
                    {
                        result.Add(new TopicExpression(fullPath, TopicExpressionTypes.CONTAINS));
                    }

                    lst = new List<string>();
                }
                else
                {
                    lst.Add(str);
                }

                i++;
            }

            if(lst.Any())
            {
                var fullPath = string.Join(".", lst);
                if (result.Any())
                {
                    result.Add(new TopicExpression(fullPath, TopicExpressionTypes.ENDWITH));
                }
                else
                {
                    result.Add(new TopicExpression(fullPath, TopicExpressionTypes.EQUAL));
                }
            }

            return result;
        }
    }
}
