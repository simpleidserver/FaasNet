using FaasNet.Runtime.Attributes;
using FaasNet.Runtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.Runtime.Factories
{
    public class ConfigurationFactory
    {
        public static Configuration New(Type type)
        {
            var funcInfo = type.GetCustomAttributes(typeof(FuncInfoAttribute), false).First() as FuncInfoAttribute;
            var configuration = new Configuration(funcInfo.ApiName, funcInfo.Version);
            configuration.Parameters = BuildParameters(type);
            return configuration;
        }

        private static ICollection<ConfigurationParameter> BuildParameters(Type type)
        {
            var result = new List<ConfigurationParameter>();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                var isGenericList = IsGenericList(propertyType);
                var parameter = new ConfigurationParameter(property.Name, isGenericList ? "array" : propertyType.Name.ToLowerInvariant());
                if (isGenericList)
                {
                    var genericArg = propertyType.GetGenericArguments().First();
                    parameter.Parameters = BuildParameters(genericArg);
                }

                var translations = (TranslationAttribute[])property.GetCustomAttributes(typeof(TranslationAttribute), false);
                foreach (var translation in translations)
                {
                    parameter.Translations.Add(new ConfigurationTranslation(translation.Language, translation.Translation));
                }

                result.Add(parameter);
            }

            return result;
        }

        private static bool IsGenericList(Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>));
        }
    }
}
