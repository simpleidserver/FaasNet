using System.Collections;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace FaasNet.Runtime.Serializer
{
    public class SkipEmptyGraphObjectVisitor : ChainedObjectGraphVisitor
    {
        public SkipEmptyGraphObjectVisitor(IObjectGraphVisitor<IEmitter> nextVisitor) : base(nextVisitor)
        {
        }

        public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            if (value.Value != null && typeof(IEnumerable).IsAssignableFrom(value.Value.GetType()))
            { 
                var enumerableObject = (IEnumerable)value.Value;
                if (!enumerableObject.GetEnumerator().MoveNext())
                {
                    return false;
                }
            }

            if (value.Value == null || string.IsNullOrWhiteSpace(value.Value.ToString()))
            {
                return false;
            }

            return base.EnterMapping(key, value, context);
        }
    }
}
