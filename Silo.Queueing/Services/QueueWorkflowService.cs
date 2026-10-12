namespace Silo.Queueing;

internal sealed class QueueWorkflowService(
    QueueDbContext dbContext,
    IDbContextFactory<QueueDbContext> dbContextFactory,
    IPublishEndpoint publishEndpoint,
    QueueTaskHandlerResolver handlerResolver) : IQueueWorkflowService
{
    public async Task<Guid> SubmitAsync(
        QueueWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);
        foreach (var task in request.Tasks)
        {
            handlerResolver.EnsureRegistered(task.TaskType);
        }

        var idempotencyHash = Hash(request.IdempotencyKey);
        var requestHash = Hash(JsonSerializer.Serialize(
            request.Tasks.Select(x => new { x.TaskType, Payload = x.Payload })));

        var existing = await dbContext.Workflows
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.IdempotencyKeyHash == idempotencyHash, cancellationToken);
        if (existing is not null)
        {
            EnsureSameRequest(existing, requestHash);
            return existing.Id;
        }

        var now = DateTimeOffset.UtcNow;
        var workflow = new QueueWorkflowRecord
        {
            Id = Guid.NewGuid(),
            IdempotencyKeyHash = idempotencyHash,
            RequestHash = requestHash,
            Status = QueueWorkflowState.Pending,
            CurrentTaskIndex = 0,
            CreatedAt = now,
            UpdatedAt = now,
            Tasks = request.Tasks.Select((task, index) => new QueueTaskRecord
            {
                Id = Guid.NewGuid(),
                Index = index,
                TaskType = task.TaskType,
                Payload = task.Payload.GetRawText(),
                Status = QueueTaskState.Pending
            }).ToList()
        };

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        try
        {
            dbContext.Workflows.Add(workflow);
            await publishEndpoint.Publish(
                new WorkflowSubmitted(workflow.Id, workflow.Tasks.Count),
                cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return workflow.Id;
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(cancellationToken);
            dbContext.ChangeTracker.Clear();

            await using var freshContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            var concurrentSubmission = await freshContext.Workflows
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.IdempotencyKeyHash == idempotencyHash, cancellationToken);
            if (concurrentSubmission is null)
            {
                throw;
            }

            EnsureSameRequest(concurrentSubmission, requestHash);
            return concurrentSubmission.Id;
        }
    }

    public async Task<QueueWorkflowStatus?> GetStatusAsync(
        Guid workflowId,
        CancellationToken cancellationToken = default)
    {
        var workflow = await dbContext.Workflows
            .AsNoTracking()
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == workflowId, cancellationToken);

        return workflow is null
            ? null
            : new QueueWorkflowStatus(
                workflow.Id,
                workflow.Status,
                workflow.CurrentTaskIndex,
                workflow.CreatedAt,
                workflow.UpdatedAt,
                workflow.StartedAt,
                workflow.CompletedAt,
                workflow.Tasks.OrderBy(x => x.Index)
                    .Select(task => new QueueTaskStatus(
                        task.Id,
                        task.Index,
                        task.TaskType,
                        task.Status,
                        task.AttemptCount,
                        task.StartedAt,
                        task.CompletedAt,
                        task.LastErrorCode,
                        task.LastErrorMessage))
                    .ToArray());
    }

    public async Task ResumeAsync(Guid workflowId, CancellationToken cancellationToken = default)
    {
        var workflow = await dbContext.Workflows
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == workflowId, cancellationToken)
            ?? throw new KeyNotFoundException($"Workflow '{workflowId}' was not found.");

        if (workflow.Status != QueueWorkflowState.Failed)
        {
            throw new InvalidOperationException("Only failed workflows can be resumed.");
        }

        var task = workflow.Tasks.SingleOrDefault(x => x.Index == workflow.CurrentTaskIndex)
            ?? throw new InvalidOperationException("The failed workflow has no incomplete task to resume.");

        task.Status = QueueTaskState.Pending;
        task.CompletedAt = null;
        task.LeaseUntil = null;
        task.NextAttemptAt = null;
        workflow.Status = QueueWorkflowState.Pending;
        workflow.CompletedAt = null;
        workflow.UpdatedAt = DateTimeOffset.UtcNow;

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        await publishEndpoint.Publish(new WorkflowResumeRequested(workflow.Id), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private static void ValidateRequest(QueueWorkflowRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.IdempotencyKey);
        if (request.IdempotencyKey.Length > 200)
        {
            throw new ArgumentException("Idempotency keys may not exceed 200 characters.", nameof(request));
        }

        ArgumentNullException.ThrowIfNull(request.Tasks);
        if (request.Tasks.Count is < 1 or > 100)
        {
            throw new ArgumentException("A workflow must contain between 1 and 100 tasks.", nameof(request));
        }

        foreach (var task in request.Tasks)
        {
            ArgumentNullException.ThrowIfNull(task);
            ArgumentException.ThrowIfNullOrWhiteSpace(task.TaskType);
            if (task.TaskType.Length > 200)
            {
                throw new ArgumentException("Task types may not exceed 200 characters.", nameof(request));
            }

            if (task.Payload.ValueKind == JsonValueKind.Undefined)
            {
                throw new ArgumentException("Task payloads must be valid JSON values.", nameof(request));
            }
        }
    }

    private static string Hash(string value)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }

    private static void EnsureSameRequest(QueueWorkflowRecord existing, string requestHash)
    {
        if (!CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(existing.RequestHash),
                Convert.FromHexString(requestHash)))
        {
            throw new IdempotencyConflictException(
                "The idempotency key has already been used for a different workflow request.");
        }
    }
}

/// <summary>Thrown when an idempotency key is reused for a different request.</summary>
public sealed class IdempotencyConflictException(string message) : InvalidOperationException(message);
