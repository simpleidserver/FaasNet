using Newtonsoft.Json.Serialization;
using NJsonSchema;
using System.Collections.Generic;

namespace FaasNet.Runtime.JSchemas
{
    public class FaasNetReferenceResolver : IReferenceResolver
    {
        private Dictionary<string, object> _dic;
        private Dictionary<FaasNetJsonSchema, string> _unprocessedJsonSchemaDic;
        private Dictionary<KeyValuePair<JsonSchemaProperty, bool>, string> _unprocessedJsonSchemaPropertyDic;

        public FaasNetReferenceResolver()
        {
            _dic = new Dictionary<string, object>();
            _unprocessedJsonSchemaDic = new Dictionary<FaasNetJsonSchema, string>();
            _unprocessedJsonSchemaPropertyDic = new Dictionary<KeyValuePair<JsonSchemaProperty, bool>, string>();
        }

        public virtual void AddReference(object context, string reference, object value)
        {   
            _dic.Add(reference, value);
            var schema = value as FaasNetJsonSchema;
            if (schema != null)
            {
                ResolveUnprocessedJsonSchema(reference, value as FaasNetJsonSchema);
                ResolveUnprocessedJsonSchemaProperty(reference, value as FaasNetJsonSchema);
            }
        }

        public string GetReference(object context, object value)
        {
            throw new System.NotImplementedException();
        }

        public bool IsReferenced(object context, object value)
        {
            throw new System.NotImplementedException();
        }

        public object ResolveReference(object context, string reference)
        {
            if(!_dic.ContainsKey(reference))
            {
                return null;
            }

            return _dic[reference];
        }

        public void AddUnprocessedJsonSchema(FaasNetJsonSchema schema, string reference)
        {
            _unprocessedJsonSchemaDic.Add(schema, reference);
        }

        public void AddUnprocessedJsonSchemaProperty(JsonSchemaProperty property, string reference, bool isArray = false)
        {
            _unprocessedJsonSchemaPropertyDic.Add(new KeyValuePair<JsonSchemaProperty, bool>(property, isArray), reference);
        }

        private void ResolveUnprocessedJsonSchema(string reference, FaasNetJsonSchema value)
        {
            foreach(var kvp in _unprocessedJsonSchemaDic)
            {
                if (kvp.Value == reference)
                {
                    kvp.Key.Reference = value;
                }
            }
        }

        private void ResolveUnprocessedJsonSchemaProperty(string reference, FaasNetJsonSchema value)
        {
            foreach (var kvp in _unprocessedJsonSchemaPropertyDic)
            {
                if (kvp.Value == reference)
                {
                    if (kvp.Key.Value)
                    {
                        kvp.Key.Key.Items.Add(value);
                    }
                    else
                    {
                        kvp.Key.Key.Reference = value;
                    }
                }
            }
        }
    }
}
