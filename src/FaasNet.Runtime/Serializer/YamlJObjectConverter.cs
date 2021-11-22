using Newtonsoft.Json.Linq;
using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace FaasNet.Runtime.Serializer
{
    public class YamlJObjectConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(JObject);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var jObj = new JObject();
            var mappingStart = parser.Current as MappingStart;
            if (mappingStart == null)
            {
                return jObj;
            }

            parser.MoveNext();
            do
            {
                var scalarKey = parser.Current as Scalar;
                var key = scalarKey.Value;
                parser.MoveNext();
                var scalarValue = parser.Current as Scalar;
                var value = scalarValue.Value;
                parser.MoveNext();
                jObj.Add(key, value);
            }
            while (parser.Current.GetType() != typeof(MappingEnd));
            parser.MoveNext();
            return jObj;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var jObj = value as JObject;
            emitter.Emit(new MappingStart(null, null, true, MappingStyle.Block));
            foreach(var record in jObj)
            {
                emitter.Emit(new Scalar(record.Key));
                emitter.Emit(new Scalar(record.Value.ToString()));
            }

            emitter.Emit(new MappingEnd());
        }
    }
}
