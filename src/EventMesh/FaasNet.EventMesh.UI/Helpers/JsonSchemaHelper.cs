using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace FaasNet.EventMesh.UI.Helpers
{
    public static class JsonSchemaHelper
    {
        public static string GenerateSample(string jsonSchema)
        {
            try
            {
                var jschema = JsonSchema.Parse(jsonSchema);
                if (jschema == null) return "{}";
                return Generate(jschema).ToString();
            }
            catch
            {
                return "{}";
            }
        }

        private static JToken Generate(JsonSchema jsonSchema)
        {
            JToken output;
            switch (jsonSchema.Type)
            {
                case JsonSchemaType.Object:
                    var jObject = new JObject();
                    if (jsonSchema.Properties != null)
                    {
                        foreach (var prop in jsonSchema.Properties)
                        {
                            jObject.Add(TranslateNameToJson(prop.Key), Generate(prop.Value));
                        }
                    }
                    output = jObject;
                    break;
                case JsonSchemaType.Array:
                    var jArray = new JArray();
                    foreach (var item in jsonSchema.Items)
                    {
                        jArray.Add(Generate(item));
                    }
                    output = jArray;
                    break;

                case JsonSchemaType.String:
                    output = new JValue("sample");
                    break;
                case JsonSchemaType.Float:
                    output = new JValue(1.0);
                    break;
                case JsonSchemaType.Integer:
                    output = new JValue(1);
                    break;
                case JsonSchemaType.Boolean:
                    output = new JValue(false);
                    break;
                case JsonSchemaType.Null:
                    output = JValue.CreateNull();
                    break;

                default:
                    output = null;
                    break;

            }


            return output;
        }
        private static string TranslateNameToJson(string name)
        {
            return name.Substring(0, 1).ToLower() + name.Substring(1);
        }
    }
}
