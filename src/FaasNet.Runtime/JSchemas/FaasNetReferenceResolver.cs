using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Reflection;

namespace FaasNet.Runtime.JSchemas
{
    public class FaasNetReferenceResolver : IReferenceResolver
    {
        private class UnprocessedObject
        {
            public UnprocessedObject(object obj, bool isArray)
            {
                Obj = obj;
                IsArray = isArray;
            }

            public object Obj { get; set; }
            public bool IsArray { get; set; }
        }

        private Dictionary<string, object> _dic;
        private Dictionary<UnprocessedObject, string> _unprocessedObjects;

        public FaasNetReferenceResolver()
        {
            _dic = new Dictionary<string, object>();
            _unprocessedObjects = new Dictionary<UnprocessedObject, string>();
        }

        public virtual void AddReference(object context, string reference, object value)
        {   
            _dic.Add(reference, value);
            ResolveUnprocessedObject(reference, value);
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

        public void AddUnprocessedObject(object obj, string reference, bool isArray = false)
        {
            _unprocessedObjects.Add(new UnprocessedObject(obj, isArray), reference);
        }

        private void ResolveUnprocessedObject(string reference, object value)
        {
            var col = typeof(ICollection<>);
            foreach(var kvp in _unprocessedObjects)
            {
                if(kvp.Value == reference)
                {
                    var unprocessedObj = kvp.Key;
                    var obj = kvp.Key.Obj;
                    var referenceType = kvp.Key.Obj.GetType().GetProperty("Reference");
                    var itemsType = kvp.Key.Obj.GetType().GetProperty("Items");
                    if(kvp.Key.IsArray)
                    {
                        var items = itemsType.GetValue(obj);
                        var addMethod = items.GetType().GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
                        addMethod.Invoke(items, new object[] { value });
                    }
                    else
                    {
                        referenceType.SetValue(obj, value);
                    }
                }
            }
        }
    }
}
