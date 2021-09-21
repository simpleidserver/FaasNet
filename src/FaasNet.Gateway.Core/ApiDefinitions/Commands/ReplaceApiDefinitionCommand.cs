using MediatR;
using System.Collections.Generic;

namespace FaasNet.Gateway.Core.ApiDefinitions.Commands
{
    public class ReplaceApiDefinitionCommand : IRequest<bool>
    {
        public ReplaceApiDefinitionCommand()
        {
            Operations = new List<ReplaceApiOperation>();
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public ICollection<ReplaceApiOperation> Operations { get; set; }
    }

    public class ReplaceApiOperation
    {
        public ReplaceApiOperation()
        {
            Functions = new List<ReplaceApiFunction>();
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public ICollection<ReplaceApiFunction> Functions { get; set; }
    }

    public class ReplaceApiFunction
    {
        public string Function { get; set; }
        public string SerializedConfiguration { get; set; }
        public ICollection<ReplaceApiSequenceFlow> Flows { get; set; }
    }

    public class ReplaceApiSequenceFlow
    {
        public string TargetRef { get; set; }
    }
}
