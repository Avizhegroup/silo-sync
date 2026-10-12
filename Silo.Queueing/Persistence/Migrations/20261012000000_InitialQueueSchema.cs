namespace Silo.Queueing.Persistence.Migrations;

[DbContext(typeof(QueueDbContext))]
[Migration("20261012000000_InitialQueueSchema")]
public sealed class InitialQueueSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            CREATE TABLE [InboxState] (
                [Id] bigint NOT NULL IDENTITY,
                [MessageId] uniqueidentifier NOT NULL,
                [ConsumerId] uniqueidentifier NOT NULL,
                [LockId] uniqueidentifier NOT NULL,
                [RowVersion] rowversion NULL,
                [Received] datetime2 NOT NULL,
                [ReceiveCount] int NOT NULL,
                [ExpirationTime] datetime2 NULL,
                [Consumed] datetime2 NULL,
                [Delivered] datetime2 NULL,
                [LastSequenceNumber] bigint NULL,
                CONSTRAINT [PK_InboxState] PRIMARY KEY ([Id]),
                CONSTRAINT [AK_InboxState_MessageId_ConsumerId] UNIQUE ([MessageId], [ConsumerId])
            );
            CREATE TABLE [OutboxState] (
                [OutboxId] uniqueidentifier NOT NULL,
                [LockId] uniqueidentifier NOT NULL,
                [RowVersion] rowversion NULL,
                [Created] datetime2 NOT NULL,
                [Delivered] datetime2 NULL,
                [LastSequenceNumber] bigint NULL,
                CONSTRAINT [PK_OutboxState] PRIMARY KEY ([OutboxId])
            );
            CREATE TABLE [QueueWorkflows] (
                [Id] uniqueidentifier NOT NULL,
                [IdempotencyKeyHash] nvarchar(64) NOT NULL,
                [RequestHash] nvarchar(64) NOT NULL,
                [Status] nvarchar(24) NOT NULL,
                [CurrentTaskIndex] int NOT NULL,
                [CreatedAt] datetimeoffset NOT NULL,
                [UpdatedAt] datetimeoffset NOT NULL,
                [StartedAt] datetimeoffset NULL,
                [CompletedAt] datetimeoffset NULL,
                [RowVersion] rowversion NOT NULL,
                CONSTRAINT [PK_QueueWorkflows] PRIMARY KEY ([Id])
            );
            CREATE TABLE [QueueWorkflowSagas] (
                [CorrelationId] uniqueidentifier NOT NULL,
                [CurrentState] nvarchar(64) NOT NULL,
                [CurrentTaskIndex] int NOT NULL,
                [TaskCount] int NOT NULL,
                [CreatedAt] datetimeoffset NOT NULL,
                [RowVersion] rowversion NOT NULL,
                CONSTRAINT [PK_QueueWorkflowSagas] PRIMARY KEY ([CorrelationId])
            );
            CREATE TABLE [OutboxMessage] (
                [SequenceNumber] bigint NOT NULL IDENTITY,
                [EnqueueTime] datetime2 NULL,
                [SentTime] datetime2 NOT NULL,
                [Headers] nvarchar(max) NULL,
                [Properties] nvarchar(max) NULL,
                [InboxMessageId] uniqueidentifier NULL,
                [InboxConsumerId] uniqueidentifier NULL,
                [OutboxId] uniqueidentifier NULL,
                [MessageId] uniqueidentifier NOT NULL,
                [ContentType] nvarchar(256) NOT NULL,
                [MessageType] nvarchar(max) NOT NULL,
                [Body] nvarchar(max) NOT NULL,
                [ConversationId] uniqueidentifier NULL,
                [CorrelationId] uniqueidentifier NULL,
                [InitiatorId] uniqueidentifier NULL,
                [RequestId] uniqueidentifier NULL,
                [SourceAddress] nvarchar(256) NULL,
                [DestinationAddress] nvarchar(256) NULL,
                [ResponseAddress] nvarchar(256) NULL,
                [FaultAddress] nvarchar(256) NULL,
                [ExpirationTime] datetime2 NULL,
                CONSTRAINT [PK_OutboxMessage] PRIMARY KEY ([SequenceNumber]),
                CONSTRAINT [FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId]
                    FOREIGN KEY ([InboxMessageId], [InboxConsumerId])
                    REFERENCES [InboxState] ([MessageId], [ConsumerId]),
                CONSTRAINT [FK_OutboxMessage_OutboxState_OutboxId]
                    FOREIGN KEY ([OutboxId]) REFERENCES [OutboxState] ([OutboxId])
            );
            CREATE TABLE [QueueWorkflowTasks] (
                [Id] uniqueidentifier NOT NULL,
                [WorkflowId] uniqueidentifier NOT NULL,
                [Index] int NOT NULL,
                [TaskType] nvarchar(200) NOT NULL,
                [Payload] nvarchar(max) NOT NULL,
                [Status] nvarchar(24) NOT NULL,
                [AttemptCount] int NOT NULL,
                [StartedAt] datetimeoffset NULL,
                [CompletedAt] datetimeoffset NULL,
                [LeaseUntil] datetimeoffset NULL,
                [NextAttemptAt] datetimeoffset NULL,
                [LastErrorCode] nvarchar(200) NULL,
                [LastErrorMessage] nvarchar(2000) NULL,
                [RowVersion] rowversion NOT NULL,
                CONSTRAINT [PK_QueueWorkflowTasks] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_QueueWorkflowTasks_QueueWorkflows_WorkflowId]
                    FOREIGN KEY ([WorkflowId]) REFERENCES [QueueWorkflows] ([Id]) ON DELETE CASCADE
            );
            CREATE INDEX [IX_InboxState_Delivered] ON [InboxState] ([Delivered]);
            CREATE INDEX [IX_OutboxMessage_EnqueueTime] ON [OutboxMessage] ([EnqueueTime]);
            CREATE INDEX [IX_OutboxMessage_ExpirationTime] ON [OutboxMessage] ([ExpirationTime]);
            CREATE UNIQUE INDEX [IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber]
                ON [OutboxMessage] ([InboxMessageId], [InboxConsumerId], [SequenceNumber])
                WHERE [InboxMessageId] IS NOT NULL AND [InboxConsumerId] IS NOT NULL;
            CREATE UNIQUE INDEX [IX_OutboxMessage_OutboxId_SequenceNumber]
                ON [OutboxMessage] ([OutboxId], [SequenceNumber]) WHERE [OutboxId] IS NOT NULL;
            CREATE INDEX [IX_OutboxState_Created] ON [OutboxState] ([Created]);
            CREATE UNIQUE INDEX [IX_QueueWorkflows_IdempotencyKeyHash]
                ON [QueueWorkflows] ([IdempotencyKeyHash]);
            CREATE UNIQUE INDEX [IX_QueueWorkflowTasks_WorkflowId_Index]
                ON [QueueWorkflowTasks] ([WorkflowId], [Index]);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DROP TABLE [QueueWorkflowTasks];
            DROP TABLE [QueueWorkflowSagas];
            DROP TABLE [QueueWorkflows];
            DROP TABLE [OutboxMessage];
            DROP TABLE [OutboxState];
            DROP TABLE [InboxState];
            """);
    }
}
