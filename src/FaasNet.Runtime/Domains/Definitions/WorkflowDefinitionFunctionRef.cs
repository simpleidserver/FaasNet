using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

namespace FaasNet.Runtime.Domains.Definitions
{
    public class WorkflowDefinitionFunctionRef
    {
        /// <summary>
        /// Name of the referenced function.
        /// </summary>
        public string ReferenceName { get; set; }
        /// <summary>
        /// Arguments (inputs) to be passed to the referenced function.
        /// </summary>
        [YamlIgnore]
        public string ArgumentsStr { get; set; }
        /// <summary>
        /// Used if function type is graphsql. String containing valid GraphSQL selection set.
        /// </summary>
        public string SelectionSet { get; set; }
        public JObject Arguments
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ArgumentsStr))
                {
                    return null;
                }

                return JObject.Parse(ArgumentsStr);
            }
            set
            {
                ArgumentsStr = value.ToString();
            }
        }
    }
}
