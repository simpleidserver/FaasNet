using FaasNet.Runtime.Builders;
using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Serializer;
using System.Linq;
using Xunit;

namespace FaasNet.Runtime.Tests
{
    public class RuntimeSerializerFixture
    {
        [Fact]
        public void When_Serialize_And_Deserialize_WorkflowDefinition()
        {
            // ARRANGE
            var workflowDefinition = WorkflowDefinitionBuilder.New("greeting", 1, "name", "description")
                // .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .StartsWith(o => o.Operation().SetActionMode(WorkflowDefinitionActionModes.Parallel).AddAction("Greet",
                (act) => {
                    act.SetFunctionRef("greetingFunction", "{ 'name' : '$.person.name' }").SetActionDataFilter(string.Empty, "$.person.message", "$.result");
                }).AddAction("SecondGreet", (act) => { }).End())
                // .Then(o => o.Operation().SetActionMode(WorkflowDefinitionActionModes.Sequential).AddAction("Greet2", (act) => { }).End())
                .Build();
            var serializer = new RuntimeSerializer();
            var yaml = serializer.SerializeYaml(workflowDefinition);
            // ARRANGE
            var deserialized = serializer.DeserializeYaml(yaml);
            // ASSERT
            var state = deserialized.States.First() as WorkflowDefinitionOperationState;
            var firstAction = state.Actions.First();
            var lastAction = state.Actions.Last();
            Assert.Equal(WorkflowDefinitionStateTypes.Operation, state.Type);
            Assert.Equal(WorkflowDefinitionActionModes.Parallel, state.ActionMode);
            Assert.Equal("Greet", firstAction.Name);
            Assert.Equal("greetingFunction", firstAction.FunctionRef.RefName);
            Assert.Equal("{\r\n  \"name\": \"$.person.name\"\r\n}", firstAction.FunctionRef.ArgumentsStr);
            Assert.Equal("$.person.message", firstAction.ActionDataFilter.ToStateData);
            Assert.Equal("$.result", firstAction.ActionDataFilter.Results);
            Assert.Equal("SecondGreet", lastAction.Name);
        }
    }
}
