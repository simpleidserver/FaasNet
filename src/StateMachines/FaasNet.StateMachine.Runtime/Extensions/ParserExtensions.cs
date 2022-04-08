using FaasNet.StateMachine.Runtime.Serializer;
using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace FaasNet.StateMachine.Runtime.Extensions
{
    public static class ParserExtensions
    {
        public static ICollection<TreeNode> Extract(this IParser parser, Type end)
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
                        TreeNode obj;
                        if(parser.Current.GetType()  == typeof(MappingStart))
                        {
                            obj = new TreeNode
                            {
                                Type = TreeNodeTypes.OBJECT
                            };
                            obj.Children = Extract(parser, typeof(MappingEnd));
                        }
                        else
                        {
                            var scalar = parser.Current as Scalar;
                            obj = new TreeNode
                            {
                                Type = TreeNodeTypes.PROPERTY,
                                Value = scalar.Value
                            };
                            parser.MoveNext();
                        }

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
    }
}
