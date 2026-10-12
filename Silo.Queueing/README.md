# Silo.Queueing

`Silo.Queueing` is the reusable, durable workflow queue library for ASP.NET Core
services in Silo. It owns workflow persistence and ordering, MassTransit saga
orchestration, RabbitMQ delivery, bounded retry scheduling, recovery, and
workflow/task status. Consuming APIs provide only task handlers for their own
business operations.

## Requirements and infrastructure

- .NET 9.
- SQL Server reachable through the configured queue connection string.
- RabbitMQ reachable through the configured host and virtual host.
- RabbitMQ's **rabbitmq_delayed_message_exchange** plugin, used by MassTransit
  for delayed redelivery after transient infrastructure failures.
- One or more application `IQueueTaskHandler` registrations.

The queue library uses its own `QueueDbContext`; it does not require changes to
an application's business `DbContext`. RabbitMQ messages are persistent and
queue declarations are durable. SQL Server stores workflows, ordered tasks,
attempts, saga progress, inbox records, and transactional outbox messages.

## Add the library to an API

Reference `Silo.Queueing` (or its published NuGet package) and make the single
infrastructure registration call:

```csharp
builder.Services.AddCompanyQueueing(builder.Configuration);
builder.Services.AddHttpClient("ExternalApi");
builder.Services.AddScoped<IQueueTaskHandler, ExternalApiTaskHandler>();
```

Register one handler per task type. Multiple handlers are supported; task type
names must be unique and are validated when the host starts. Do not register
RabbitMQ consumers, MassTransit, saga state machines, or queue persistence in
the consuming API.

Apply queue database migrations as an explicit deployment/startup operation
before processing traffic:

```csharp
await app.Services
    .GetRequiredService<IQueueDatabaseMigrator>()
    .MigrateAsync();
```

Migration application uses EF Core `MigrateAsync`; it applies pending
migrations and does not call `EnsureCreated`, drop tables, or recreate an
existing database. In production, applying migrations in a controlled
deployment step is recommended.

## Configuration

Settings are strongly typed and bound from one `CompanyQueueing` section.
Required values are validated at host startup. Keep credentials and production
connection strings in deployment configuration or a secret provider, not in
source control.

```json
{
  "CompanyQueueing": {
    "RabbitMq": {
      "Host": "localhost",
      "VirtualHost": "/",
      "Username": "",
      "Password": ""
    },
    "Database": {
      "ConnectionString": ""
    },
    "Processing": {
      "MaxRetryCount": 5,
      "InitialRetryDelaySeconds": 5,
      "MaxRetryDelaySeconds": 300,
      "RequestTimeoutSeconds": 60,
      "ConcurrentWorkflows": 4
    }
  }
}
```

Configuration values and meanings:

| Setting | Meaning |
| --- | --- |
| `RabbitMq:Host` | RabbitMQ DNS name or address. |
| `RabbitMq:VirtualHost` | Broker virtual host. |
| `RabbitMq:Username`, `Password` | Broker credentials. |
| `Database:ConnectionString` | SQL Server connection used exclusively for queue data. |
| `Processing:MaxRetryCount` | Retries after the first attempt; zero disables handler retries. |
| `Processing:InitialRetryDelaySeconds` | First retry delay. |
| `Processing:MaxRetryDelaySeconds` | Upper bound for exponential retry delay. |
| `Processing:RequestTimeoutSeconds` | Cancellation timeout passed to task handlers. |
| `Processing:ConcurrentWorkflows` | Maximum task messages processed concurrently by this API host. |

The initial retry delay must not exceed the maximum retry delay.
`ConcurrentWorkflows` limits consumer concurrency per API host; separate API
instances each enforce their own limit.

## Submit, inspect, and resume workflows

Inject `IQueueWorkflowService`; callers do not publish MassTransit messages
directly. Requests are durable and idempotent. The idempotency key is hashed
before storage. Repeating an identical request with the same key returns the
original workflow ID; using the key for a different request throws
`IdempotencyConflictException`.

```csharp
var workflowId = await queueWorkflowService.SubmitAsync(
    new QueueWorkflowRequest
    {
        IdempotencyKey = requestId,
        Tasks =
        [
            new QueueTaskRequest
            {
                TaskType = "external.update",
                Payload = JsonSerializer.SerializeToElement(
                    new { recordId = 42, status = "ready" })
            },
            new QueueTaskRequest
            {
                TaskType = "external.notify",
                Payload = JsonSerializer.SerializeToElement(
                    new { recordId = 42 })
            }
        ]
    },
    cancellationToken);
```

The request supports 1–100 ordered tasks. `SubmitAsync` validates task types
against registered handlers and stores both the workflow and initial outgoing
message through the SQL transactional bus outbox. It returns the workflow ID
only after the database transaction commits.

`GetStatusAsync(workflowId)` returns `null` for an unknown ID; otherwise the
status includes current task index, workflow lifecycle/timestamps, and ordered
task statuses, attempt counts, and safe last error details.
`ResumeAsync(workflowId)` resumes a failed workflow at its first incomplete
task. It does not restart tasks already completed or reset the attempt history.
Resuming an unknown or non-failed workflow raises an exception.

## Implement application task handlers

`IQueueTaskHandler` is the only queue extension point applications need. Task
types resolve through explicitly registered handlers; payload values are never
used as CLR type names or executable code.

```csharp
public sealed class ExternalApiTaskHandler(IHttpClientFactory clients)
    : IQueueTaskHandler
{
    public string TaskType => "external.update";

    public async Task<QueueTaskResult> ExecuteAsync(
        QueueTaskContext context,
        CancellationToken cancellationToken)
    {
        using var client = clients.CreateClient("ExternalApi");
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "application-specific/path");
        request.Headers.Add("Idempotency-Key", context.IdempotencyKey);
        request.Content = JsonContent.Create(context.Payload);

        using var response = await client.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return QueueTaskResult.Success();
        }

        var retryable = (int)response.StatusCode == 429 ||
                        (int)response.StatusCode >= 500;
        return QueueTaskResult.Failure(
            "The external operation was rejected.",
            retryable,
            $"HTTP_{(int)response.StatusCode}");
    }
}
```

Replace the placeholder URI and request construction with the consuming
application's actual endpoint contract, DTOs, authentication, and response
validation. Do not put endpoint-specific or business-specific logic in this
library. Handlers should return only safe error text: exception messages and
upstream response bodies may contain secrets or personal information. The
stable workflow/task `IdempotencyKey` should be forwarded when the external
service supports idempotency.

The handler receives the payload as a cloned `JsonElement`, the zero-based task
index, one-based attempt number, workflow/task IDs, and stable task
idempotency key. Its cancellation token is cancelled after
`RequestTimeoutSeconds` or when the consumer is stopping. Handlers must honor
that token.

## Execution, retries, and recovery

MassTransit persists saga state in SQL Server and drives one task at a time.
After a task succeeds, its result and the saga event are saved through the EF
bus outbox before the saga dispatches the next task. The task consumer performs
its external handler call outside the outbox transaction. Duplicate task
deliveries are protected by persisted task state and row-version concurrency;
duplicate or stale completion events are correlated with the workflow and task
index and cannot advance a later task.

Task behavior:

1. The saga dispatches only the current task.
2. The consumer claims that task and persists its attempt and lease.
3. A successful result marks that task completed and advances the persisted
   workflow cursor.
4. A permanent failure, or a retryable failure after the retry limit, marks
   both task and workflow failed and blocks later tasks.
5. A retryable failure stores a safe error detail and schedules a delayed
   retry using capped exponential backoff.

`MaxRetryCount` means retries after the initial attempt. For example, five
retries allow at most six handler executions. The retry due time is persisted with the `Retrying` state in SQL. The recovery
service republishes due work; duplicate publications are safe because task
claims and workflow cursors are persisted and concurrency-protected. This scan
only dispatches due work and does not busy-loop. Handler exceptions and
timeouts are treated as retryable; handlers should return `Failure(...,
retryable: false)` for permanent application failures.

The recovery hosted service periodically scans SQL for expired task leases and
due retries and republishes their execution messages. It only dispatches tasks
whose saga is in the running state, so startup recovery cannot bypass saga
orchestration. SQL state, not the recovery timer, is the source of truth. This
covers process/host restarts and lost or redelivered broker messages.
RabbitMQ's delayed exchange plugin is required for MassTransit delayed
redelivery of transient infrastructure failures.

The library does not claim exactly-once external HTTP execution. A process can
fail after a remote service has completed an operation but before SQL records
the result. Use the provided stable task idempotency key with external APIs
that support it. Otherwise, the operation may be repeated during recovery.

## Persistence and schema

The EF Core context and migration are in this project assembly. The initial
migration creates:

- `QueueWorkflows` and `QueueWorkflowTasks`, including unique idempotency and
  per-workflow task-order constraints.
- `QueueWorkflowSagas` for MassTransit state machine progress.
- `InboxState`, `OutboxMessage`, and `OutboxState` for duplicate delivery and
  transactional outgoing messages.
- Row-version columns for optimistic concurrency of queue records.

Applications can apply migrations through `IQueueDatabaseMigrator` as shown
above. The service is also registered in DI for deployment tooling. Do not
assume RabbitMQ stores workflow progress; the SQL Server queue database is
authoritative.

## Testing and build

Build the library only:

```sh
dotnet build Silo.Queueing/Silo.Queueing.csproj
```

Run the isolated test project (it is intentionally not added to the main
solution):

```sh
dotnet test Silo.Queueing.Tests/Silo.Queueing.Tests.csproj
```

Tests cover service registration and configuration validation, EF migration
discovery/schema contents, and saga task ordering/failure blocking with
MassTransit's in-memory test harness. A live RabbitMQ + SQL Server deployment
test is not included; broker outage, SQL contention, and remote HTTP behavior
must additionally be validated in the consuming deployment environment.
