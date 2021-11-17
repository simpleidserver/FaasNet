using System;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace FaasNet.Runtime.Serializer
{
    public class YamlEnumConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type.IsEnum;
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var scalar = parser.Current as Scalar;
            var names = Enum.GetNames(type);
            var result = Convert.ChangeType(Enum.Parse(type, names.First(n => n.ToLowerInvariant() == scalar.Value)), type);
            return result;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            emitter.Emit(new Scalar(value.ToString().ToLowerInvariant()));
        }
    }
}
