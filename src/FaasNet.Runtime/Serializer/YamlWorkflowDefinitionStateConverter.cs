using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace FaasNet.Runtime.Serializer
{
    public class YamlWorkflowDefinitionStateConverter : IYamlTypeConverter
    {
        private static Dictionary<WorkflowDefinitionStateTypes, Type> MappingEnumToType = new Dictionary<WorkflowDefinitionStateTypes, Type>
        {
            { WorkflowDefinitionStateTypes.Callback, typeof(WorkflowDefinitionCallbackState) },
            { WorkflowDefinitionStateTypes.Event, typeof(WorkflowDefinitionEventState) },
            { WorkflowDefinitionStateTypes.ForEach, typeof(WorkflowDefinitionForeachState) },
            { WorkflowDefinitionStateTypes.Inject, typeof(WorkflowDefinitionInjectState) },
            { WorkflowDefinitionStateTypes.Operation, typeof(WorkflowDefinitionOperationState) },
            { WorkflowDefinitionStateTypes.Switch, typeof(WorkflowDefinitionSwitchState) }
        };

        public bool Accepts(Type type)
        {
            return typeof(BaseWorkflowDefinitionState).IsAssignableFrom(type);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var nodes = parser.Extract(typeof(MappingEnd));
            var operation = nodes.First(n => n.Key == "type");
            var stateEnum = (WorkflowDefinitionStateTypes)Enum.Parse(typeof(WorkflowDefinitionStateTypes), Enum.GetNames(typeof(WorkflowDefinitionStateTypes)).First(n => n.ToLowerInvariant() == operation.Value));
            var stateType = MappingEnumToType[stateEnum];
            return Build(nodes, stateType);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            throw new NotImplementedException();
        }

        private object Build(ICollection<TreeNode> nodes, Type type)
        {
            var result = Activator.CreateInstance(type);
            foreach(var node in nodes)
            {
                var property = type.GetProperties().FirstOrDefault(p => p.Name.ToLowerInvariant() == node.Key.ToLowerInvariant());
                if (property == null)
                {
                    continue;
                }

                if(property.PropertyType == typeof(JObject))
                {
                    var jObj = node.Build().SelectToken(node.Key);
                    if (jObj != null && (jObj is JObject))
                    {
                        property.SetValue(result, jObj);
                    }
                    continue;
                }

                switch(node.Type)
                {
                    case TreeNodeTypes.PROPERTY:
                        TrySetPrimitive(result, property, node.Value);
                        TrySetEnum(result, property, node.Value);
                        TrySetStr(result, property, node.Value);
                        break;
                    case TreeNodeTypes.OBJECT:
                        var value = Build(node.Children, property.PropertyType);
                        property.SetValue(result, value);
                        break;
                    case TreeNodeTypes.ARRAY:
                        var genericType = property.PropertyType.GetGenericArguments().First();
                        var lstType = typeof(List<>).MakeGenericType(genericType);
                        var lst = Activator.CreateInstance(lstType);
                        var addMethod = lstType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
                        foreach (var child in node.Children)
                        {
                            var v = Build(child.Children, genericType);
                            addMethod.Invoke(lst, new object[] { v });
                        }

                        property.SetValue(result, lst);
                        break;
                }
            }

            return result;
        }

        private static bool TrySetPrimitive(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.IsEnum)
            {
                return false;
            }

            var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
            var value = converter.ConvertFrom(str);
            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool TrySetStr(object obj, PropertyInfo propertyInfo, string str)
        {
            if (propertyInfo.PropertyType != typeof(string))
            {
                return false;
            }

            propertyInfo.SetValue(obj, str);
            return true;
        }

        private static bool TrySetEnum(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!propertyInfo.PropertyType.IsEnum)
            {
                return false;
            }

            var name = Enum.GetNames(propertyInfo.PropertyType).First(n => n.ToLowerInvariant() == str);
            var value = Enum.Parse(propertyInfo.PropertyType, name);
            propertyInfo.SetValue(obj, value);
            return true;
        }
    }
}
