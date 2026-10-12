namespace Silo.Queueing;

internal sealed record WorkflowSubmitted(Guid WorkflowId, int TaskCount);
internal sealed record WorkflowResumeRequested(Guid WorkflowId);
internal sealed record ExecuteWorkflowTask(Guid WorkflowId, int TaskIndex);
internal sealed record WorkflowTaskSucceeded(Guid WorkflowId, int TaskIndex);
internal sealed record WorkflowTaskFailed(Guid WorkflowId, int TaskIndex);
