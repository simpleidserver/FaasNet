using System;

namespace FaasNet.Gateway.Core.Domains
{
    public class  ApiDefinitionSequenceFlow : ICloneable
    {
        public string SourceRef { get; set; }
        public string TargetRef { get; set; }

        public static ApiDefinitionSequenceFlow Create(string sourceRef, string targetRef)
        {
            return new ApiDefinitionSequenceFlow
            {
                SourceRef = sourceRef,
                TargetRef = targetRef
            };
        }

        public object Clone()
        {
            return new ApiDefinitionSequenceFlow
            {
                SourceRef = SourceRef,
                TargetRef = TargetRef
            };
        }
    }
}
