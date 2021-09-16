using FaasNet.Runtime.Attributes;
using System.Collections.Generic;

namespace FaasNet.Runtime.Transform
{
    [FuncInfo("Transform", "v1")]
    public class TransformConfiguration
    {
        [Translation("fr", "Règles de mapping")]
        [Translation("en", "Mappings")]
        public List<Mapping> Mappings { get; set; }
    }
}