using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FaasNet.Runtime.Serializer
{
    public class TreeNode
    {
        public TreeNode()
        {
            Children = new List<TreeNode>();
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public ICollection<TreeNode> Children { get; set; }
        public TreeNodeTypes Type { get; set; }

        public JObject Build()
        {
            var result = new JObject();
            Build(result);
            return result;
        }

        protected void Build(JObject jObj)
        {
            switch (Type)
            {
                case TreeNodeTypes.PROPERTY:
                    jObj.Add(Key, Value);
                    break;
                case TreeNodeTypes.OBJECT:
                    var childJObj = new JObject();
                    foreach (var child in Children)
                    {
                        child.Build(childJObj);
                    }

                    jObj.Add(Key, childJObj);
                    break;
                case TreeNodeTypes.ARRAY:
                    var jarr = new JArray();
                    foreach (var child in Children)
                    {
                        jarr.Add(child.Build());
                    }

                    jObj.Add(Key, jarr);
                    break;
            }
        }
    }
}
