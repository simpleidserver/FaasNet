namespace FaasNet.Runtime.Domains
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
        public string ArgumentsStr { get; set; }
        /// <summary>
        /// Used if function type is graphsql. String containing valid GraphSQL selection set.
        /// </summary>
        public string SelectionSet { get; set; }
    }
}
