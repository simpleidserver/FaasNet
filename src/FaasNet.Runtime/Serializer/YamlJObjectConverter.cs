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
            throw new NotImplementedException();
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
