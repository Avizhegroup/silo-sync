namespace Silo.Queueing;

internal sealed class QueueWorkflowStateMachine : MassTransitStateMachine<QueueWorkflowSaga>
{
    public State Running { get; private set; } = null!;
    public State Failed { get; private set; } = null!;
    public State Completed { get; private set; } = null!;

    public Event<WorkflowSubmitted> Submitted { get; private set; } = null!;
    public Event<WorkflowResumeRequested> ResumeRequested { get; private set; } = null!;
    public Event<WorkflowTaskSucceeded> TaskSucceeded { get; private set; } = null!;
    public Event<WorkflowTaskFailed> TaskFailed { get; private set; } = null!;
    public Event<Fault<ExecuteWorkflowTask>> TaskFaulted { get; private set; } = null!;

    public QueueWorkflowStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => Submitted, e => e.CorrelateById(c => c.Message.WorkflowId));
        Event(() => ResumeRequested, e => e.CorrelateById(c => c.Message.WorkflowId));
        Event(() => TaskSucceeded, e => e.CorrelateById(c => c.Message.WorkflowId));
        Event(() => TaskFailed, e => e.CorrelateById(c => c.Message.WorkflowId));
        Event(() => TaskFaulted, e => e.CorrelateById(c => c.Message.Message.WorkflowId));

        Initially(
            When(Submitted)
                .Then(context =>
                {
                    context.Saga.CurrentTaskIndex = 0;
                    context.Saga.TaskCount = context.Message.TaskCount;
                    context.Saga.CreatedAt = DateTimeOffset.UtcNow;
                })
                .TransitionTo(Running)
                .Publish(context => new ExecuteWorkflowTask(context.Saga.CorrelationId, 0)));

        During(Running,
            When(TaskSucceeded)
                .If(context => context.Message.TaskIndex == context.Saga.CurrentTaskIndex, then => then
                    .Then(context => context.Saga.CurrentTaskIndex++)
                    .IfElse(
                        context => context.Saga.CurrentTaskIndex >= context.Saga.TaskCount,
                        completed => completed.TransitionTo(Completed),
                        next => next
                            .Publish(context => new ExecuteWorkflowTask(
                                context.Saga.CorrelationId,
                                context.Saga.CurrentTaskIndex)))),
            When(TaskFailed)
                .If(context => context.Message.TaskIndex == context.Saga.CurrentTaskIndex,
                    then => then.TransitionTo(Failed)),
            When(TaskFaulted)
                .Then(context =>
                {
                    context.Saga.CurrentTaskIndex = context.Message.Message.TaskIndex;
                })
                .TransitionTo(Failed));

        During(Failed,
            When(ResumeRequested)
                .TransitionTo(Running)
                .Publish(context => new ExecuteWorkflowTask(
                    context.Saga.CorrelationId,
                    context.Saga.CurrentTaskIndex)));
    }
}
