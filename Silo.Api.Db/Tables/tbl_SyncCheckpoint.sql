CREATE TABLE [dbo].[tbl_SyncCheckpoint] (
    [fld_SourceKey]            NVARCHAR (100) NOT NULL,
    [fld_LastCheckpointValue]  DATETIME       NULL,
    [fld_UpdatedDate]          DATETIME       NULL,
    CONSTRAINT [PK_tbl_SyncCheckpoint] PRIMARY KEY CLUSTERED ([fld_SourceKey] ASC)
);
