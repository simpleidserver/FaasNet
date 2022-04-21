using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Exceptions;
using System;
using System.Linq;
using Xunit;

namespace FaasNet.StateMachine.Core.Tests.Domains
{
    public class StateMachineInstanceAggregateFixture
    {
        [Theory]
        [InlineData(StateMachineInstanceStatus.ACTIVE)]
        [InlineData(StateMachineInstanceStatus.TERMINATE)]
        public void When_Reactivate_StateMachineInstanceWithInvalidStatus_Then_Exception_Is_Thrown(StateMachineInstanceStatus status)
        {
            // ARRANGE
            var stateMachineInstanceAggregate = new StateMachineInstanceAggregate
            {
                Status = status
            };

            // ACT
            var ex = Assert.Throws<BusinessException>(() => stateMachineInstanceAggregate.Reactivate());

            // ASSERT
            Assert.NotNull(ex);
            Assert.Equal(string.Format("state machine instance with status '{0}' cannot be reactivated", Enum.GetName(typeof(StateMachineInstanceStatus), status)), ex.Message);
        }

        [Fact]
        public void When_Reactivate_FailedStateMachineInstance_Then_HistoryIsCorrect()
        {
            // ARRANGE
            var stateMachineInstanceAggregate = StateMachineInstanceAggregate.Create("technical", "defId", "defName", "defDescription", 0, "vpn", "rootTopic", "def");
            var instance = stateMachineInstanceAggregate.AddState("defId");
            stateMachineInstanceAggregate.ErrorState(instance.Id, "exception");

            // ACT
            stateMachineInstanceAggregate.Reactivate();

            // ASSERT
            var orderedHistories = stateMachineInstanceAggregate.Histories.OrderBy(h => h.StartDateTime);
            Assert.Equal(3, orderedHistories.Count());
            Assert.Equal(StateMachineInstanceStatus.ACTIVE, orderedHistories.ElementAt(0).Status);
            Assert.Equal(StateMachineInstanceStatus.FAILED, orderedHistories.ElementAt(1).Status);
            Assert.Equal(StateMachineInstanceStatus.ACTIVE, orderedHistories.ElementAt(2).Status);
            Assert.NotNull(orderedHistories.ElementAt(0).EndDateTime);
            Assert.NotNull(orderedHistories.ElementAt(1).EndDateTime);
            Assert.Null(orderedHistories.ElementAt(2).EndDateTime);
            Assert.Equal(StateMachineInstanceStatus.ACTIVE, stateMachineInstanceAggregate.Status);
        }
    }
}
