using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace FaasNet.Runtime.JSchemas
{
    public class JsonSchemaConverter : JsonConverter
    {
        private const string REFERENCE_NAME = "$ref";

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FaasNetJsonSchema);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var resolver = serializer.ReferenceResolver as FaasNetReferenceResolver;
            var jObject = JObject.Load(reader);
            var result = new FaasNetJsonSchema();
            serializer.Populate(jObject.CreateReader(), result);
            UpdateReferenceJsonSchema(reader, resolver, result, jObject);
            UpdateReferenceJsonSchemaProperties(reader, resolver, result, jObject);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // base.WriteJson(writer, value, serializer);
        }

        public static void UpdateReferenceJsonSchema(JsonReader reader, FaasNetReferenceResolver resolver, FaasNetJsonSchema schema, JObject jObj)
        {
            if (jObj.ContainsKey(REFERENCE_NAME))
            {
                var reference = jObj.SelectToken(REFERENCE_NAME).ToString();
                var resolvedReference = resolver.ResolveReference(reader, reference) as FaasNetJsonSchema;
                if (resolvedReference != null)
                {
                    schema.Reference = resolvedReference;
                }
                else
                {
                    resolver.AddUnprocessedJsonSchema(schema, reference);
                }
            }
            else
            {
                resolver.AddReference(reader, FormatPath(reader.Path), schema);
            }
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
                        resolver.AddUnprocessedJsonSchemaProperty(schemaProperty, reference, isArray);
                    }
                }
            }
        }

        public static string FormatPath(string path)
        {
            return $"#/{path.Replace('.', '/')}";
        }
    }
}
