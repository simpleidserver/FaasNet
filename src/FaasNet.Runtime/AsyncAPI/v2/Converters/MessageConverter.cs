using FaasNet.Runtime.AsyncAPI.v2.Models;
using FaasNet.Runtime.JSchemas;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace FaasNet.Runtime.AsyncAPI.v2.Converters
{
    public class MessageConverter : JsonConverter
    {
        private const string REFERENCE_NAME = "$ref";

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IMessage);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var resolver = serializer.ReferenceResolver as AsyncApiReferenceResolver;
            var jObject = JObject.Load(reader);
            if (jObject.ContainsKey("$ref"))
            {
                var messageReference = new MessageReference();
                var reference = jObject.SelectToken(REFERENCE_NAME).ToString();
                var resolvedReference = resolver.ResolveReference(reader, reference) as Message;
                if (resolvedReference != null)
                {
                    messageReference.Reference = resolvedReference;
                }
                else
                {
                    resolver.AddUnprocessedMessageReference(messageReference, reference);
                }

                return messageReference;
            }

            var result = new Message();
            serializer.Populate(jObject.CreateReader(), result);
            resolver.AddReference(reader, JsonSchemaConverter.FormatPath(reader.Path), result);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

        }
    }
}
