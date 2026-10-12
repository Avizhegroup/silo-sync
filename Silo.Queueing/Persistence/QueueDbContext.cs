namespace Silo.Queueing;

public sealed class QueueDbContext(DbContextOptions<QueueDbContext> options)
    : SagaDbContext(options)
{
    internal DbSet<QueueWorkflowRecord> Workflows => Set<QueueWorkflowRecord>();
    internal DbSet<QueueTaskRecord> Tasks => Set<QueueTaskRecord>();

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new QueueWorkflowSagaMap();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<QueueWorkflowRecord>(entity =>
        {
            entity.ToTable("QueueWorkflows");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.IdempotencyKeyHash).IsUnique();
            entity.Property(x => x.IdempotencyKeyHash).HasMaxLength(64).IsRequired();
            entity.Property(x => x.RequestHash).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(24);
            entity.Property(x => x.RowVersion).IsRowVersion();
            entity.HasMany(x => x.Tasks).WithOne(x => x.Workflow)
                .HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QueueTaskRecord>(entity =>
        {
            entity.ToTable("QueueWorkflowTasks");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.WorkflowId, x.Index }).IsUnique();
            entity.Property(x => x.TaskType).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Payload).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(24);
            entity.Property(x => x.RowVersion).IsRowVersion();
            entity.Property(x => x.LastErrorCode).HasMaxLength(200);
            entity.Property(x => x.LastErrorMessage).HasMaxLength(2000);
        });

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
