using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
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
            var nodes = Extract(parser, typeof(MappingEnd));
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
                    property.SetValue(result, jObj);
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

        private ICollection<TreeNode> Extract(IParser parser, Type end, bool ignoreFirstElement = false)
        {
            var result = new List<TreeNode>();
            if (parser.Current.GetType() != typeof(MappingStart))
            {
                return result;
            }

            parser.MoveNext();
            do
            {
                var propertyName = parser.Current as Scalar;
                parser.MoveNext();
                var scalarPropertyValue = parser.Current as Scalar;
                var sequencePropertyValue = parser.Current as SequenceStart;
                var mappingPropertyValue = parser.Current as MappingStart;
                var record = new TreeNode
                {
                    Key = propertyName.Value
                };
                if (scalarPropertyValue != null)
                {
                    record.Value = scalarPropertyValue.Value;
                    record.Type = TreeNodeTypes.PROPERTY;
                    parser.MoveNext();
                }
                else if (sequencePropertyValue != null)
                {
                    parser.MoveNext();
                    var children = new List<TreeNode>();
                    do
                    {
                        var obj = new TreeNode
                        {
                            Type = TreeNodeTypes.OBJECT
                        };
                        obj.Children = Extract(parser, typeof(MappingEnd));
                        children.Add(obj);
                    }
                    while (parser.Current.GetType() != typeof(SequenceEnd));
                    parser.MoveNext();
                    record.Children = children;
                    record.Type = TreeNodeTypes.ARRAY;
                }
                else if (mappingPropertyValue != null)
                {
                    record.Children = Extract(parser, typeof(MappingEnd));
                    record.Type = TreeNodeTypes.OBJECT;
                }

                result.Add(record);
            }
            while (parser.Current.GetType() != end);
            parser.MoveNext();
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
