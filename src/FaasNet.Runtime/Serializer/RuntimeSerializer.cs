using FaasNet.Runtime.Domains.Definitions;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FaasNet.Runtime.Serializer
{
    public class RuntimeSerializer
    {
        public string SerializeYaml(WorkflowDefinitionAggregate workflowDefinition)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new YamlEnumConverter())
                .WithTypeConverter(new YamlJObjectConverter())
                .WithEmissionPhaseObjectGraphVisitor(args => new SkipEmptyGraphObjectVisitor(args.InnerVisitor))
                .Build();
            var yaml = serializer.Serialize(workflowDefinition);
            return yaml;
        }

        public WorkflowDefinitionAggregate DeserializeYaml(string yaml)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new YamlWorkflowDefinitionStateConverter())
                .WithTypeConverter(new YamlEnumConverter())
                .WithTypeConverter(new YamlJObjectConverter())
                .Build();
            var result = deserializer.Deserialize<WorkflowDefinitionAggregate>(yaml);
            if (result.States.Any())
            {
                result.States.First().IsRootState = true;
            }

            return result;
        }
    }
}
