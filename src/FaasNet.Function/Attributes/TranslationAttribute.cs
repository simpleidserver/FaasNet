using System;

namespace FaasNet.Function.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class TranslationAttribute : Attribute
    {
        public TranslationAttribute(string language, string translation)
        {
            Language = language;
            Translation = translation;
        }

        public string Language { get; private set; }
        public string Translation { get; private set; }
    }
}
