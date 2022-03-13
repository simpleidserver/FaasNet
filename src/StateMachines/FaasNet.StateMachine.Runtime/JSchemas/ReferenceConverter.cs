using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace FaasNet.StateMachine.Runtime.JSchemas
{
    public class ReferenceConverter : JsonConverter
    {
        protected const string REFERENCE_NAME = "$ref";

        protected class ReferenceParseResult
        {
            public ReferenceParseResult(object obj, JObject jObj)
            {
                Obj = obj;
                JObj = jObj;
            }

            public object Obj { get; }
            public JObject JObj { get; }
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var parseJson = ParseJson(reader, objectType, existingValue, serializer);
            return parseJson.Obj;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        protected static string FormatPath(string path)
        {
            return $"#/{path.Replace('.', '/')}";
        }

        protected virtual ReferenceParseResult ParseJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object result = null;
            var resolver = serializer.ReferenceResolver as FaasNetReferenceResolver;
            var jObject = JObject.Load(reader);
            if (!TryConvertReference(reader, jObject, objectType, resolver, out result))
            {
                result = Activator.CreateInstance(objectType);
                serializer.Populate(jObject.CreateReader(), result);
                resolver.AddReference(reader, FormatPath(reader.Path), result);
            }
            else
            {
                serializer.Populate(jObject.CreateReader(), result);
            }

            return new ReferenceParseResult(result, jObject);
        }

        protected bool TryConvertReference(JsonReader reader, JObject jObj, Type objectType, FaasNetReferenceResolver resolver, out object result)
        {
            result = null;
            if (!jObj.ContainsKey(REFERENCE_NAME))
            {
                return false;
            }

            result = Activator.CreateInstance(objectType);
            var reference = jObj.SelectToken(REFERENCE_NAME).ToString();
            var resolvedReference = resolver.ResolveReference(reader, reference);
            var property = objectType.GetProperty("Reference");
            if (resolvedReference != null)
            {
                property.SetValue(result, resolvedReference);
            }
            else
            {
                resolver.AddUnprocessedObject(result, reference);
            }

            return true;
        }
    }
}
