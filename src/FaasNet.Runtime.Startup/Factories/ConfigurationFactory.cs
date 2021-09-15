using FaasNet.Runtime.Startup.Attributes;
using FaasNet.Runtime.Startup.Models;
using System;
using System.Linq;

namespace FaasNet.Runtime.Startup.Factories
{
    public class ConfigurationFactory
    {
        public static Configuration New(Type type)
        {
            var funcInfo = type.GetCustomAttributes(typeof(FuncInfoAttribute), false).First() as FuncInfoAttribute;
            var configuration = new Configuration(funcInfo.ApiName, funcInfo.Version);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var parameter = new ConfigurationParameter(property.Name, property.PropertyType.Name);
                var translations = (TranslationAttribute[])property.GetCustomAttributes(typeof(TranslationAttribute), false);
                foreach (var translation in translations)
                {
                    parameter.Translations.Add(new ConfigurationTranslation(translation.Language, translation.Translation));
                }

                configuration.Parameters.Add(parameter);
            }

            return configuration;
        }
    }
}
