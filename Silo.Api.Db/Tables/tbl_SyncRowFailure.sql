CREATE TABLE [dbo].[tbl_SyncRowFailure] (
    [fld_SyncRowFailureId]  INT             IDENTITY (1, 1) NOT NULL,
    [fld_SyncRunLogId]      INT             NULL,
    [fld_SourceKey]         NVARCHAR (100)  NULL,
    [fld_RowKey]            NVARCHAR (200)  NULL,
    [fld_ErrorCategory]     NVARCHAR (100)  NULL,
    [fld_ErrorMessage]      NVARCHAR (MAX)  NULL,
    [fld_RawPayload]        NVARCHAR (MAX)  NULL,
    [fld_AttemptCount]      INT             NOT NULL DEFAULT (0),
    [fld_LastAttemptAt]     DATETIME        NULL,
    [fld_NextAttemptAt]     DATETIME        NULL,
    [fld_Status]            NVARCHAR (30)   NULL,
    [fld_ResolvedDate]      DATETIME        NULL,
    CONSTRAINT [PK_tbl_SyncRowFailure] PRIMARY KEY CLUSTERED ([fld_SyncRowFailureId] ASC)
);
