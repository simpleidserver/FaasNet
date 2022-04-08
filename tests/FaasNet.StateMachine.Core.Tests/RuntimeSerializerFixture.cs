using FaasNet.StateMachine.Runtime.Builders;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Serializer;
using System.Linq;
using Xunit;

namespace FaasNet.StateMachine.Core.Tests
{
    public class RuntimeSerializerFixture
    {
        [Fact]
        public void When_Serialize_And_Deserialize_WorkflowDefinition()
        {
            // ARRANGE
            var workflowDefinition = StateMachineDefinitionBuilder.New("greeting", 1, "name", "description", "default")
                .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .AddConsumedEvent("GreetingEvent", "https://github.com/cloudevents/spec/pull", "com.github.pull.create", "greetingTopic")
                .StartsWith(o => o.Operation().SetActionMode(StateMachineDefinitionActionModes.Parallel).AddAction("Greet",
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
            var state = deserialized.States.First() as StateMachineDefinitionOperationState;
            var firstAction = state.Actions.First();
            var lastAction = state.Actions.Last();
            Assert.Equal(StateMachineDefinitionStateTypes.Operation, state.Type);
            Assert.Equal(StateMachineDefinitionActionModes.Parallel, state.ActionMode);
            Assert.Equal("Greet", firstAction.Name);
            Assert.Equal("greetingFunction", firstAction.FunctionRef.RefName);
            Assert.Equal("{\r\n  \"name\": \"$.person.name\"\r\n}", firstAction.FunctionRef.ArgumentsStr);
            Assert.Equal("$.person.message", firstAction.ActionDataFilter.ToStateData);
            Assert.Equal("$.result", firstAction.ActionDataFilter.Results);
            Assert.Equal("SecondGreet", lastAction.Name);
        }
    }
}
