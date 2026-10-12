namespace Silo.Queueing;

internal sealed class QueueWorkflowSaga : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    public int CurrentTaskIndex { get; set; }
    public int TaskCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

internal sealed class QueueWorkflowSagaMap : SagaClassMap<QueueWorkflowSaga>
{
    protected override void Configure(
        EntityTypeBuilder<QueueWorkflowSaga> entity,
        ModelBuilder model)
    {
        entity.ToTable("QueueWorkflowSagas");
        entity.HasKey(x => x.CorrelationId);
        entity.Property(x => x.CurrentState).HasMaxLength(64).IsRequired();
        entity.Property(x => x.RowVersion).IsRowVersion();
    }
}
