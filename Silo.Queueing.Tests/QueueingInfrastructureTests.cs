using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using MassTransit.Testing;

namespace Silo.Queueing.Tests;

public sealed class QueueingInfrastructureTests
{
    [Fact]
    public async Task Registration_exposes_validated_settings_and_services()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCompanyQueueing(CreateConfiguration());

        await using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });

        await using var scope = provider.CreateAsyncScope();
        var settings = provider.GetRequiredService<IOptions<CompanyQueueingOptions>>().Value;
        Assert.Equal("localhost", settings.RabbitMq.Host);
        Assert.Equal(5, settings.Processing.MaxRetryCount);
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IQueueWorkflowService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IQueueDatabaseMigrator>());
        Assert.NotNull(provider.GetRequiredService<IBusControl>());
    }

    [Fact]
    public void Registration_rejects_missing_credentials_when_settings_are_resolved()
    {
        var values = ConfigurationValues();
        values.Remove("CompanyQueueing:RabbitMq:Password");
        var services = new ServiceCollection();
        services.AddCompanyQueueing(new ConfigurationBuilder().AddInMemoryCollection(values).Build());
        using var provider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(
            () => provider.GetRequiredService<IOptions<CompanyQueueingOptions>>().Value);
    }

    [Fact]
    public void Queue_context_contains_queue_saga_and_outbox_migrations()
    {
        var options = new DbContextOptionsBuilder<QueueDbContext>()
            .UseSqlServer("Server=(local);Database=Queue;Integrated Security=True;TrustServerCertificate=True")
            .Options;
        using var context = new QueueDbContext(options);

        Assert.Contains("20261012000000_InitialQueueSchema", context.Database.GetMigrations());
        var schema = context.Database.GenerateCreateScript();
        Assert.Contains("[QueueWorkflows]", schema);
        Assert.Contains("[QueueWorkflowTasks]", schema);
        Assert.Contains("[QueueWorkflowSagas]", schema);
        Assert.Contains("[OutboxMessage]", schema);
        Assert.Contains("[InboxState]", schema);
    }

    [Fact]
    public void Handler_resolver_supports_multiple_task_types_and_rejects_duplicates()
    {
        var resolver = new QueueTaskHandlerResolver(
        [
            new TestTaskHandler("first"),
            new TestTaskHandler("second")
        ]);

        Assert.Equal("first", resolver.Resolve("first").TaskType);
        Assert.Equal("second", resolver.Resolve("second").TaskType);
        Assert.Throws<InvalidOperationException>(() => new QueueTaskHandlerResolver(
        [
            new TestTaskHandler("duplicate"),
            new TestTaskHandler("duplicate")
        ]));
    }

    [Fact]
    public async Task Saga_dispatches_tasks_in_order_and_does_not_advance_after_failure()
    {
        var services = new ServiceCollection();
        services.AddMassTransitTestHarness(registration =>
        {
            registration.AddSagaStateMachine<QueueWorkflowStateMachine, QueueWorkflowSaga>()
                .InMemoryRepository();
        });

        await using var provider = services.BuildServiceProvider(true);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness<QueueWorkflowStateMachine, QueueWorkflowSaga>();
        var workflowId = Guid.NewGuid();

        await harness.Bus.Publish(new WorkflowSubmitted(workflowId, 5));
        Assert.True(await harness.Published.Any<ExecuteWorkflowTask>(
            message => message.Context.Message.WorkflowId == workflowId &&
                       message.Context.Message.TaskIndex == 0));

        await harness.Bus.Publish(new WorkflowTaskSucceeded(workflowId, 0));
        Assert.True(await harness.Published.Any<ExecuteWorkflowTask>(
            message => message.Context.Message.WorkflowId == workflowId &&
                       message.Context.Message.TaskIndex == 1));

        for (var index = 1; index < 5; index++)
        {
            await harness.Bus.Publish(new WorkflowTaskSucceeded(workflowId, index));
            Assert.True(await sagaHarness.Consumed.Any<WorkflowTaskSucceeded>(
                message => message.Context.Message.WorkflowId == workflowId &&
                           message.Context.Message.TaskIndex == index));
            if (index < 4)
            {
                var nextIndex = index + 1;
                Assert.True(await harness.Published.Any<ExecuteWorkflowTask>(
                    message => message.Context.Message.WorkflowId == workflowId &&
                               message.Context.Message.TaskIndex == nextIndex));
            }
        }

        Assert.True(await sagaHarness.Consumed.Any<WorkflowTaskSucceeded>());

        await harness.Stop();
    }

    [Fact]
    public async Task Failed_saga_blocks_later_tasks_and_resumes_the_failed_task()
    {
        var services = new ServiceCollection();
        services.AddMassTransitTestHarness(registration =>
        {
            registration.AddSagaStateMachine<QueueWorkflowStateMachine, QueueWorkflowSaga>()
                .InMemoryRepository();
        });

        await using var provider = services.BuildServiceProvider(true);
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness<QueueWorkflowStateMachine, QueueWorkflowSaga>();
        var workflowId = Guid.NewGuid();

        await harness.Bus.Publish(new WorkflowSubmitted(workflowId, 5));
        Assert.True(await sagaHarness.Consumed.Any<WorkflowSubmitted>(
            message => message.Context.Message.WorkflowId == workflowId));
        Assert.True(await harness.Published.Any<ExecuteWorkflowTask>(
            message => message.Context.Message.WorkflowId == workflowId &&
                       message.Context.Message.TaskIndex == 0));
        await harness.Bus.Publish(new WorkflowTaskFailed(workflowId, 0));
        Assert.True(await sagaHarness.Consumed.Any<WorkflowTaskFailed>(
            message => message.Context.Message.WorkflowId == workflowId));

        await harness.Bus.Publish(new WorkflowTaskSucceeded(workflowId, 0));
        Assert.True(await sagaHarness.Consumed.Any<WorkflowTaskSucceeded>(
            message => message.Context.Message.WorkflowId == workflowId));
        Assert.False(await harness.Published.Any<ExecuteWorkflowTask>(
            message => message.Context.Message.WorkflowId == workflowId &&
                       message.Context.Message.TaskIndex == 1));

        await harness.Bus.Publish(new WorkflowResumeRequested(workflowId));
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        Assert.Equal(2, harness.Published.Select<ExecuteWorkflowTask>().Count(
            message => message.Context.Message.WorkflowId == workflowId &&
                       message.Context.Message.TaskIndex == 0));

        await harness.Stop();
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder().AddInMemoryCollection(ConfigurationValues()).Build();
    }

    private static Dictionary<string, string?> ConfigurationValues()
    {
        return new Dictionary<string, string?>
        {
            ["CompanyQueueing:RabbitMq:Host"] = "localhost",
            ["CompanyQueueing:RabbitMq:VirtualHost"] = "/",
            ["CompanyQueueing:RabbitMq:Username"] = "queue-user",
            ["CompanyQueueing:RabbitMq:Password"] = "queue-password",
            ["CompanyQueueing:Database:ConnectionString"] =
                "Server=(local);Database=Queue;Integrated Security=True;TrustServerCertificate=True",
            ["CompanyQueueing:Processing:MaxRetryCount"] = "5",
            ["CompanyQueueing:Processing:InitialRetryDelaySeconds"] = "5",
            ["CompanyQueueing:Processing:MaxRetryDelaySeconds"] = "300",
            ["CompanyQueueing:Processing:RequestTimeoutSeconds"] = "60",
            ["CompanyQueueing:Processing:ConcurrentWorkflows"] = "4"
        };
    }

    private sealed class TestTaskHandler(string taskType) : IQueueTaskHandler
    {
        public string TaskType { get; } = taskType;

        public Task<QueueTaskResult> ExecuteAsync(
            QueueTaskContext context,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(QueueTaskResult.Success());
        }
    }
}
