# Silo.Queueing

`Silo.Queueing` is the dedicated .NET class-library boundary for reusable,
durable workflow queuing shared by ASP.NET Core APIs. Its intended
responsibilities are RabbitMQ transport, MassTransit setup, ordered workflow
execution, retries, SQL Server persistence, recovery, and status inspection.
Application-specific operations remain in the API that consumes the library.

## Current project status

This change establishes the .NET 9 class library and adds it to `Silo.slnx`.
It is a project scaffold only: queue services, consumers, handlers, message
contracts, MassTransit/RabbitMQ setup, persistence entities, EF Core migrations,
and automated tests have not yet been implemented. The API shapes and examples
below describe the intended contract from issue #17; they must not be treated as
available runtime functionality until those components are implemented.

No broker, database, or external API is required to build this empty scaffold.
The full solution does not need to be built to validate this project.

## Design boundary

The library is intended to own all reusable queue infrastructure:

- The public workflow submission, status, and resume service.
- Queue-specific public request, result, context, and status models.
- Handler lookup and internal task consumers.
- MassTransit/RabbitMQ configuration and durable message handling.
- Saga orchestration and strict sequential processing.
- SQL Server queue persistence, EF Core mappings and migrations.
- Retry scheduling, failure tracking, and workflow recovery.
- Inbox/outbox and duplicate-message handling where supported by the selected
  MassTransit architecture.
- Startup configuration and validation, logging, and diagnostics integration.

Consuming applications should own only application-specific task handlers and
their dependencies, such as external API clients, authentication, request and
response DTOs, and business validation. They should not need to define queue
consumers or saga state machines, configure MassTransit or RabbitMQ, or persist
workflow state in their business `DbContext`.

The library must use explicit handler registrations to map task type names to
`IQueueTaskHandler` implementations. It must not construct types or execute
code selected from untrusted task payloads.

## Intended consumer contract

The target integration is one registration call in the consuming API:

```csharp
builder.Services.AddCompanyQueueing(builder.Configuration);
```

The library is intended to expose an `IQueueWorkflowService` with operations
equivalent to:

```csharp
Task<Guid> SubmitAsync(
    QueueWorkflowRequest request,
    CancellationToken cancellationToken = default);

Task<QueueWorkflowStatus?> GetStatusAsync(
    Guid workflowId,
    CancellationToken cancellationToken = default);

Task ResumeAsync(
    Guid workflowId,
    CancellationToken cancellationToken = default);
```

These signatures are a design target, not implemented methods in the current
scaffold. Submission should return an identifier only after durable acceptance.
Requests should include an explicit idempotency key so a retried API request
does not create a second workflow.

The intended task-handler extension point is:

```csharp
public interface IQueueTaskHandler
{
    string TaskType { get; }

    Task<QueueTaskResult> ExecuteAsync(
        QueueTaskContext context,
        CancellationToken cancellationToken);
}
```

`QueueTaskContext` should carry the workflow and task identifiers, registered
task type, payload, and execution metadata. `QueueTaskResult` should represent
success or failure, safe failure details, and whether the failure is retryable.
Payloads should be validated by their application-specific handler.

Multiple handler implementations must be resolvable by distinct task types.
An application-specific handler may use `IHttpClientFactory`; endpoint details,
authentication, HTTP DTOs, and response validation belong to that handler, not
to this infrastructure library. External calls cannot be guaranteed exactly
once: where the external API supports it, handlers should pass an idempotency
key derived from the workflow/task identity.

## Intended workflow behavior

A workflow is an ordered list of tasks. The library should support at least the
five-task case and arbitrary practical task counts. Its core invariants are:

1. Execute only the first incomplete task.
2. Do not begin the next task until the current task has succeeded.
3. Never execute two tasks in the same workflow concurrently.
4. Preserve task order and do not silently skip failed tasks.
5. Allow different workflows to execute concurrently, subject to configuration.
6. Persist task attempts and state transitions so redelivery or restart cannot
   incorrectly advance a workflow.
7. Mark a workflow `Completed` only after every task has succeeded.

The intended workflow and task lifecycle is:

| Status | Meaning |
| --- | --- |
| `Pending` | Accepted durably and waiting to start. |
| `Running` | The current task is being processed. |
| `Retrying` | A retryable failure is waiting for its scheduled retry. |
| `Completed` | Every task completed successfully. |
| `Failed` | Processing stopped after a permanent failure or retry exhaustion. |

Task-level status and attempt history should identify the current task, each
task's outcome, timestamps, retry count, and safe error details. Resuming a
failed workflow should retry its current incomplete task; it must not reset
completed tasks or restart from task one.

## Intended configuration

Configuration is planned under one strongly typed `CompanyQueueing` section.
The precise options and defaults remain subject to implementation and startup
validation.

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

The configuration provider should be supplied by the application, deployment
environment, or secret store. Credentials and production endpoints must not be
committed in source control or embedded in code. Invalid or missing required
settings should fail validation during application startup, rather than
producing partially configured queue services.

Retry behavior should use bounded exponential backoff, honor the configured
maximum attempt count and delay, and distinguish retryable from permanent
failures. Exhausted workflows should remain durably `Failed` until explicitly
resumed or otherwise administered; retries must not busy-loop.

## Persistence, delivery, and recovery

The planned `QueueDbContext` belongs to this library and is independent of a
consumer's application `DbContext`. SQL Server persistence should cover
workflow identity and status, ordered task data, task attempts and errors,
timestamps, idempotency keys, and MassTransit saga state. Appropriate unique
constraints and concurrency protection are needed for idempotency and competing
updates.

RabbitMQ should use durable queues and persistent messages where supported.
MassTransit saga persistence and SQL Server are the source of truth for progress,
not process memory, static state, local files, or in-process timers. Recovery
should continue at the first incomplete task after API, worker, host, broker,
or database restarts, and should tolerate consumer crashes and message
redelivery. Error queues and dead-letter behavior should follow the selected
MassTransit/RabbitMQ configuration.

Inbox/outbox support should be configured where the chosen topology can provide
it. Duplicate submission and delivery should be safe, and state changes should
not cause a failed task to advance. External HTTP operations cannot participate
in the queue database transaction: a timeout may happen after the remote
service has processed a request. Handler-level external idempotency is preferred
when available; otherwise that uncertainty must be documented and handled
conservatively.

EF Core migrations should be shipped with the library and applied explicitly
by a consuming application or deployment process. Migration instructions and
the eventual command/project configuration should be documented when the
migrations are implemented. Startup must not drop tables or automatically run
destructive database operations.

## Intended consumer setup

After the public infrastructure is implemented, a consumer API is expected to
register it and its application-specific handlers approximately as follows:

```csharp
builder.Services.AddCompanyQueueing(builder.Configuration);
builder.Services.AddHttpClient("ExternalApi");
builder.Services.AddScoped<IQueueTaskHandler, ExternalApiTaskHandler>();
```

The API then injects `IQueueWorkflowService` to submit a `QueueWorkflowRequest`,
inspect task/workflow status, and resume a failed workflow. `ExternalApiTaskHandler`
is application code: it uses a named or typed `HttpClient`, implements the
application's authentication and endpoint-specific protocol, validates the
response, and returns a `QueueTaskResult`. The consuming API may expose its own
HTTP endpoints for those service operations; this library is not intended to
create application controllers.

## Project and validation

The project targets `net9.0` through the repository's shared build properties.
It is included in `Silo.slnx` under the Core infrastructure solution folder.

Build just this project while it remains independent of solution-wide build
issues:

```sh
dotnet build Silo.Queueing/Silo.Queueing.csproj
```

Queue-specific tests, migration commands, and an end-to-end RabbitMQ/SQL Server
example are not available in this scaffold. They should be added alongside the
future implementation rather than implied by this document.

## Scope and non-goals

This project is intended to be reusable by multiple ASP.NET Core APIs and
NuGet-packaged once its implementation and packaging are complete. It does not
choose an external API contract, move business logic into the library, guarantee
exactly-once external HTTP execution, or introduce a second broker or workflow
engine. The current change adds only the project boundary and this design
document; it does not claim the issue's queueing behavior is complete.
