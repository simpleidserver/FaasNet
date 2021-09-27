using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CLI.Helpers
{
    public static class InputParameterParser
    {
        public static bool TryParse<T>(IEnumerable<string> args, out string errorMessage, out T result)
        {
            result = Activator.CreateInstance<T>();
            errorMessage = null;
            var properties = typeof(T).GetProperties();
            foreach(var property in properties)
            {
                var attrs = property.GetCustomAttributes(false);
                if (!attrs.Any())
                {
                    continue;
                }

                var attr = attrs.FirstOrDefault(a => (a as ParameterAttribute) != null) as ParameterAttribute;
                if (attr == null)
                {
                    continue;
                }

                var value = GetValue(args, attr.Name);
                if (string.IsNullOrWhiteSpace(value) && attr.IsRequired)
                {
                    errorMessage = $"The parameter '{attr.Name}' is missing";
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    property.SetValue(result, value);
                }
                else if (!string.IsNullOrWhiteSpace(attr.DefaultValue))
                {
                    property.SetValue(result, attr.DefaultValue);
                }
            }

            return true;
        }

        private static string GetValue(IEnumerable<string> args, string name)
        {
            for (int i = 0; i < args.Count(); i++)
            {
                if (args.ElementAt(i) == name && (i + 1) < args.Count())
                {
                    return args.ElementAt(i + 1);
                }
            }

            return null;
        }
    }
}
