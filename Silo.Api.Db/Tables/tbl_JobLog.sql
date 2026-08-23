CREATE TABLE [dbo].[tbl_JobLog] (
    [fld_JobLogId]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [fld_JobLogType]     INT            NULL,
    [fld_JobLogEventId]  nvarchar(50)            NULL,
    [fld_JobLogDateTime] DATETIME       NULL,
    [fld_JobLogDate]     NVARCHAR (10)  NULL,
    [fld_JobLogTime]     NVARCHAR (5)   NULL,
    [fld_JobLogValue]    DATETIME NULL
);

