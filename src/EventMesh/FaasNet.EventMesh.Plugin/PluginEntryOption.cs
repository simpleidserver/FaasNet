using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace FaasNet.EventMesh.Plugin
{
    [DebuggerDisplay("Name = {Name}, default value = {DefaultValue}")]
    public class PluginEntryOption
    {
        public PluginEntryOption(string name, string description, string defaultValue, PropertyInfo propertyInfo)
        {
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
            PropertyInfo = propertyInfo;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string DefaultValue { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }

        public static ICollection<PluginEntryOption> Extract(Type type)
        {
            var result = new List<PluginEntryOption>();
            var instance = Activator.CreateInstance(type);
            var publicProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach(var publicProperty in publicProperties)
            {
                var optionAttr = publicProperty.GetCustomAttribute<PluginEntryOptionPropertyAttribute>();
                if (optionAttr == null) continue;
                var defaultValue = publicProperty.GetValue(instance)?.ToString();
                result.Add(new PluginEntryOption(optionAttr.Name, optionAttr.Description, defaultValue, publicProperty));
            }

            return result;
        }
    }
}
