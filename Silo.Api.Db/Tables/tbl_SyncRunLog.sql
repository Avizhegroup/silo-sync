CREATE TABLE [dbo].[tbl_SyncRunLog] (
    [fld_SyncRunLogId]   INT             IDENTITY (1, 1) NOT NULL,
    [fld_SourceKey]      NVARCHAR (100)  NULL,
    [fld_StartedAt]      DATETIME        NULL,
    [fld_FinishedAt]     DATETIME        NULL,
    [fld_RowsFetched]    INT             NULL,
    [fld_RowsSucceeded]  INT             NULL,
    [fld_RowsFailed]     INT             NULL,
    [fld_Status]         NVARCHAR (30)   NULL,
    [fld_ErrorSummary]   NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_tbl_SyncRunLog] PRIMARY KEY CLUSTERED ([fld_SyncRunLogId] ASC)
);
