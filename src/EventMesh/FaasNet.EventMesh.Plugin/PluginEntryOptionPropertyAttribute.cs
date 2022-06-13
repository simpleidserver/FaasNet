using System;

namespace FaasNet.EventMesh.Plugin
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PluginEntryOptionPropertyAttribute : Attribute
    {
        public PluginEntryOptionPropertyAttribute(string name, string description = "")
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}
