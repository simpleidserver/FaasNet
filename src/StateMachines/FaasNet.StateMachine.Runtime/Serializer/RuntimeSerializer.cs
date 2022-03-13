using FaasNet.StateMachine.Runtime.Domains.Definitions;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FaasNet.StateMachine.Runtime.Serializer
{
    public class RuntimeSerializer
    {
        public string SerializeYaml(StateMachineDefinitionAggregate workflowDefinition)
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

        public StateMachineDefinitionAggregate DeserializeYaml(string yaml)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new YamlWorkflowDefinitionStateConverter())
                .WithTypeConverter(new YamlEnumConverter())
                .WithTypeConverter(new YamlJObjectConverter())
                .Build();
            var result = deserializer.Deserialize<StateMachineDefinitionAggregate>(yaml);
            return result;
        }
    }
}
