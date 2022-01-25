using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace FaasNet.Runtime.JSchemas
{
    public class JsonSchemaConverter : ReferenceConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var resolver = serializer.ReferenceResolver as FaasNetReferenceResolver;
            var parseResult = ParseJson(reader, objectType, existingValue, serializer);
            var schema = parseResult.Obj as FaasNetJsonSchema;
            UpdateReferenceJsonSchemaProperties(reader, resolver, schema, parseResult.JObj);
            return parseResult.Obj;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public static void UpdateReferenceJsonSchemaProperties(JsonReader reader, FaasNetReferenceResolver resolver, FaasNetJsonSchema schema, JObject jObj)
        {
            if(schema.Properties == null)
            {
                return;
            }

            foreach (var kvp in schema.Properties)
            {
                var schemaProperty = kvp.Value;
                var schemaPropertyReference = jObj.SelectToken($"properties.{kvp.Key}.{REFERENCE_NAME}");
                var schemaPropertyItemsReference = jObj.SelectToken($"properties.{kvp.Key}.items.{REFERENCE_NAME}");
                if (schemaPropertyReference != null || schemaPropertyItemsReference != null)
                {
                    var reference = schemaPropertyReference == null ? schemaPropertyItemsReference.ToString() : schemaPropertyReference.ToString();
                    var isArray = schemaPropertyItemsReference != null;
                    var resolvedReference = resolver.ResolveReference(reader, reference) as FaasNetJsonSchema;
                    if (resolvedReference != null)
                    {
                        if (!schemaProperty.IsArray)
                        {
                            schemaProperty.Reference = resolvedReference;
                        }
                        else
                        {
                            schemaProperty.Items.Add(resolvedReference);
                        }
                    }
                    else
                    {
                        resolver.AddUnprocessedObject(schemaProperty, reference, isArray);
                    }
                }
            }
        }
    }
}
