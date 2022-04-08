using FaasNet.StateMachine.Runtime.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace FaasNet.StateMachine.Runtime.Serializer
{
    public class YamlJObjectConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(JObject);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var treeNodes = parser.Extract(typeof(MappingEnd));
            return Build(treeNodes);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var jObj = value as JObject;
            emitter.Emit(new MappingStart(null, null, true, MappingStyle.Block));
            foreach(var record in jObj)
            {
                emitter.Emit(new Scalar(record.Key));
                switch(record.Value.Type)
                {
                    case JTokenType.Object:
                        {
                            var val = record.Value as JObject;
                            WriteYaml(emitter, val, type);
                        }
                        break;
                    case JTokenType.String:
                        {
                            emitter.Emit(new Scalar(record.Value.ToString()));
                        }
                        break;
                }
            }

            emitter.Emit(new MappingEnd());
        }

        private JObject Build(ICollection<TreeNode> nodes)
        {
            var result = new JObject();
            foreach(var node in nodes)
            {
                switch(node.Type)
                {
                    case TreeNodeTypes.PROPERTY:
                        result.Add(node.Key, node.Value);
                        break;
                    case TreeNodeTypes.OBJECT:
                        result.Add(node.Key, Build(node.Children));
                        break;
                    case TreeNodeTypes.ARRAY:
                        var jArr = new JArray();
                        foreach(var child in node.Children)
                        {
                            jArr.Add(Build(child.Children));
                        }

                        result.Add(node.Key, jArr);
                        break;
                }
            }

            return result;
        }
    }
}
