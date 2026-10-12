namespace Silo.Queueing;

internal sealed class QueueTaskConsumer(
    QueueDbContext dbContext,
    QueueTaskHandlerResolver handlerResolver,
    IPublishEndpoint publishEndpoint,
    IOptions<CompanyQueueingOptions> options,
    ILogger<QueueTaskConsumer> logger) : IConsumer<ExecuteWorkflowTask>
{
    public const string EndpointName = "queue-workflow-task";

    public async Task Consume(ConsumeContext<ExecuteWorkflowTask> context)
    {
        var now = DateTimeOffset.UtcNow;
        var workflow = await dbContext.Workflows
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == context.Message.WorkflowId, context.CancellationToken);

        if (workflow is null ||
            workflow.Status is QueueWorkflowState.Completed or QueueWorkflowState.Failed ||
            context.Message.TaskIndex != workflow.CurrentTaskIndex)
        {
            return;
        }

        var saga = await dbContext.Sagas
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.CorrelationId == workflow.Id,
                context.CancellationToken);
        if (saga is null ||
            saga.CurrentState != "Running" ||
            saga.CurrentTaskIndex != context.Message.TaskIndex)
        {
            return;
        }

        var task = workflow.Tasks.SingleOrDefault(x => x.Index == context.Message.TaskIndex);
        if (task is null || task.Status == QueueTaskState.Completed)
        {
            return;
        }

        if (task.Status == QueueTaskState.Running && task.LeaseUntil > now)
        {
            return;
        }

        if (task.Status == QueueTaskState.Retrying && task.NextAttemptAt > now)
        {
            return;
        }

        handlerResolver.EnsureRegistered(task.TaskType);

        task.Status = QueueTaskState.Running;
        task.AttemptCount++;
        task.StartedAt ??= now;
        task.NextAttemptAt = null;
        task.LeaseUntil = now.AddSeconds(options.Value.Processing.RequestTimeoutSeconds * 2);
        workflow.Status = QueueWorkflowState.Running;
        workflow.StartedAt ??= now;
        workflow.UpdatedAt = now;

        try
        {
            await dbContext.SaveChangesAsync(context.CancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await RefreshAfterConcurrencyAsync(task, workflow, context.CancellationToken))
            {
                return;
            }

            throw;
        }

        using var payloadDocument = JsonDocument.Parse(task.Payload);
        var handlerContext = new QueueTaskContext(
            workflow.Id,
            task.Id,
            task.Index,
            task.TaskType,
            payloadDocument.RootElement.Clone(),
            task.AttemptCount,
            $"{workflow.Id:N}:{task.Id:N}");

        QueueTaskResult result;
        try
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(options.Value.Processing.RequestTimeoutSeconds));
            result = await handlerResolver.Resolve(task.TaskType)
                .ExecuteAsync(handlerContext, timeout.Token);
            if (result is null)
            {
                throw new InvalidOperationException("The task handler returned no result.");
            }
        }
        catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Queue task {TaskId} in workflow {WorkflowId} raised an exception.",
                task.Id,
                workflow.Id);
            result = QueueTaskResult.Failure("Task handler execution failed.", retryable: true);
        }

        now = DateTimeOffset.UtcNow;
        task.LeaseUntil = null;
        workflow.UpdatedAt = now;

        if (result.Succeeded)
        {
            task.Status = QueueTaskState.Completed;
            task.CompletedAt = now;
            task.LastErrorCode = null;
            task.LastErrorMessage = null;
            workflow.CurrentTaskIndex = task.Index + 1;
            if (workflow.CurrentTaskIndex >= workflow.Tasks.Count)
            {
                workflow.Status = QueueWorkflowState.Completed;
                workflow.CompletedAt = now;
            }

            await publishEndpoint.Publish(
                new WorkflowTaskSucceeded(workflow.Id, task.Index),
                context.CancellationToken);
            try
            {
                await dbContext.SaveChangesAsync(context.CancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await RefreshAfterConcurrencyAsync(task, workflow, context.CancellationToken))
                {
                    return;
                }

                throw;
            }

            return;
        }

        var retryLimit = options.Value.Processing.MaxRetryCount;
        task.LastErrorCode = Limit(result.ErrorCode, 200);
        task.LastErrorMessage = Limit(result.SafeErrorMessage ?? "Task failed.", 2000);

        if (result.Retryable && task.AttemptCount - 1 < retryLimit)
        {
            var delay = GetRetryDelay(task.AttemptCount, options.Value.Processing);
            task.Status = QueueTaskState.Retrying;
            task.NextAttemptAt = now.Add(delay);
            workflow.Status = QueueWorkflowState.Retrying;

            try
            {
                await dbContext.SaveChangesAsync(context.CancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await RefreshAfterConcurrencyAsync(task, workflow, context.CancellationToken))
                {
                    return;
                }

                throw;
            }

            return;
        }

        task.Status = QueueTaskState.Failed;
        task.CompletedAt = now;
        workflow.Status = QueueWorkflowState.Failed;
        workflow.CompletedAt = null;
        await publishEndpoint.Publish(
            new WorkflowTaskFailed(workflow.Id, task.Index),
            context.CancellationToken);
        try
        {
            await dbContext.SaveChangesAsync(context.CancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await RefreshAfterConcurrencyAsync(task, workflow, context.CancellationToken))
            {
                return;
            }

            throw;
        }
    }

    private async Task<bool> RefreshAfterConcurrencyAsync(
        QueueTaskRecord task,
        QueueWorkflowRecord workflow,
        CancellationToken cancellationToken)
    {
        await dbContext.Entry(task).ReloadAsync(cancellationToken);
        await dbContext.Entry(workflow).ReloadAsync(cancellationToken);

        return task.Status == QueueTaskState.Completed ||
               task.Status == QueueTaskState.Running && task.LeaseUntil > DateTimeOffset.UtcNow ||
               workflow.Status is QueueWorkflowState.Completed or QueueWorkflowState.Failed ||
               workflow.CurrentTaskIndex != task.Index;
    }

    private static TimeSpan GetRetryDelay(int attempt, QueueProcessingOptions settings)
    {
        var exponentialSeconds = settings.InitialRetryDelaySeconds * Math.Pow(2, Math.Max(0, attempt - 1));
        return TimeSpan.FromSeconds(Math.Min(exponentialSeconds, settings.MaxRetryDelaySeconds));
    }

    private static string? Limit(string? value, int length)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Length <= length ? value : value[..length];
    }
}
