using System;

namespace FaasNet.CLI
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute : Attribute
    {
        public ParameterAttribute(string name, bool isRequired, string defaultValue = null)
        {
            Name = name;
            IsRequired = isRequired;
            DefaultValue = defaultValue;
        }

        public string Name { get; private set; }
        public bool IsRequired { get; private set; }
        public string DefaultValue { get; private set; }
    }
}
