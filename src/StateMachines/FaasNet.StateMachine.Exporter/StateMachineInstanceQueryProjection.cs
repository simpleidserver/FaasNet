using FaasNet.EventStore;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Domains.Instances.Events;
using FaasNet.StateMachineInstance.Persistence;
using Microsoft.Extensions.Options;
using System.Threading;

namespace FaasNet.StateMachine.Exporter
{
    public class StateMachineInstanceQueryProjection : BaseQueryProjection
    {
        private readonly StateMachineExporterOptions _options;
        private readonly IStateMachineInstanceRepository _stateMachineInstanceRepository;

        public StateMachineInstanceQueryProjection(IStateMachineInstanceRepository stateMachineInstanceRepository, IEventStoreConsumer eventStoreConsumer, ISubscriptionRepository subscriptionRepository, IOptions<StateMachineExporterOptions> options) : base(eventStoreConsumer, subscriptionRepository)
        {
            _stateMachineInstanceRepository = stateMachineInstanceRepository;
            _options = options.Value;
        }

        protected override string Topic => StateMachineInstanceAggregate.TOPIC_NAME;
        protected override string GroupId => _options.GroupId;

        protected override void Project(ProjectionBuilder builder)
        {
            builder.On<StateMachineInstanceCreatedEvent>(async (evt) =>
            {
                var stateMachineInstance = new StateMachineInstanceAggregate();
                stateMachineInstance.Handle(evt);
                await _stateMachineInstanceRepository.Add(stateMachineInstance, CancellationToken.None);
                await _stateMachineInstanceRepository.SaveChanges(CancellationToken.None);
            }).On<StateCompletedEvent>(async (evt) =>
            {
                var stateMachineInstance = await _stateMachineInstanceRepository.Get(evt.AggregateId, CancellationToken.None);
                stateMachineInstance.Handle(evt);
                await _stateMachineInstanceRepository.Update(stateMachineInstance, CancellationToken.None);
                await _stateMachineInstanceRepository.SaveChanges(CancellationToken.None);
            }).On<StateBlockedEvent>(async (evt) =>
            {
                var stateMachineInstance = await _stateMachineInstanceRepository.Get(evt.AggregateId, CancellationToken.None);
                stateMachineInstance.Handle(evt);
                await _stateMachineInstanceRepository.Update(stateMachineInstance, CancellationToken.None);
                await _stateMachineInstanceRepository.SaveChanges(CancellationToken.None);
            }).On<StateEvtConsumedEvent>(async (evt) =>
            {
                var stateMachineInstance = await _stateMachineInstanceRepository.Get(evt.AggregateId, CancellationToken.None);
                stateMachineInstance.Handle(evt);
                await _stateMachineInstanceRepository.Update(stateMachineInstance, CancellationToken.None);
                await _stateMachineInstanceRepository.SaveChanges(CancellationToken.None);
            }).On<StateEvtListenedEvent>(async (evt) =>
            {
                var stateMachineInstance = await _stateMachineInstanceRepository.Get(evt.AggregateId, CancellationToken.None);
                stateMachineInstance.Handle(evt);
                await _stateMachineInstanceRepository.Update(stateMachineInstance, CancellationToken.None);
                await _stateMachineInstanceRepository.SaveChanges(CancellationToken.None);
            }).On<StateFailedEvent>(async (evt) =>
            {
                var stateMachineInstance = await _stateMachineInstanceRepository.Get(evt.AggregateId, CancellationToken.None);
                stateMachineInstance.Handle(evt);
                await _stateMachineInstanceRepository.Update(stateMachineInstance, CancellationToken.None);
                await _stateMachineInstanceRepository.SaveChanges(CancellationToken.None);
            }).On<StateInstanceCreatedEvent>(async (evt) =>
            {
                var stateMachineInstance = await _stateMachineInstanceRepository.Get(evt.AggregateId, CancellationToken.None);
                stateMachineInstance.Handle(evt);
                await _stateMachineInstanceRepository.Update(stateMachineInstance, CancellationToken.None);
                await _stateMachineInstanceRepository.SaveChanges(CancellationToken.None);
            }).On<StateMachineTerminatedEvent>(async (evt) =>
            {
                var stateMachineInstance = await _stateMachineInstanceRepository.Get(evt.AggregateId, CancellationToken.None);
                stateMachineInstance.Handle(evt);
                await _stateMachineInstanceRepository.Update(stateMachineInstance, CancellationToken.None);
                await _stateMachineInstanceRepository.SaveChanges(CancellationToken.None);
            }).On<StateProcessedEvent>(async (evt) =>
            {
                var stateMachineInstance = await _stateMachineInstanceRepository.Get(evt.AggregateId, CancellationToken.None);
                stateMachineInstance.Handle(evt);
                await _stateMachineInstanceRepository.Update(stateMachineInstance, CancellationToken.None);
                await _stateMachineInstanceRepository.SaveChanges(CancellationToken.None);
            }).On<StateStartedEvent>(async (evt) =>
            {
                var stateMachineInstance = await _stateMachineInstanceRepository.Get(evt.AggregateId, CancellationToken.None);
                stateMachineInstance.Handle(evt);
                await _stateMachineInstanceRepository.Update(stateMachineInstance, CancellationToken.None);
                await _stateMachineInstanceRepository.SaveChanges(CancellationToken.None);
            });
        }
    }
}
