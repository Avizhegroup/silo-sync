CREATE TABLE [dbo].[tbl_SyncSourceConfig] (
    [fld_SyncSourceConfigId]          INT             IDENTITY (1, 1) NOT NULL,
    [fld_SourceKey]                   NVARCHAR (100)  NOT NULL,
    [fld_DisplayName]                 NVARCHAR (200)  NULL,
    [fld_SourceType]                  NVARCHAR (50)   NULL,
    [fld_ConnectionStringEncrypted]   NVARCHAR (MAX)  NULL,
    [fld_Command]                     NVARCHAR (MAX)  NULL,
    [fld_FieldKey]                    NVARCHAR (100)  NULL,
    [fld_FieldCheck]                  NVARCHAR (100)  NULL,
    [fld_FieldOrder]                  NVARCHAR (100)  NULL,
    [fld_IntervalSeconds]             INT             NULL,
    [fld_IsEnabled]                   BIT             NOT NULL DEFAULT (1),
    [fld_CreatedBy]                   NVARCHAR (100)  NULL,
    [fld_CreatedDate]                 DATETIME        NULL,
    [fld_ModifiedBy]                  NVARCHAR (100)  NULL,
    [fld_ModifiedDate]                DATETIME        NULL,
    CONSTRAINT [PK_tbl_SyncSourceConfig] PRIMARY KEY CLUSTERED ([fld_SyncSourceConfigId] ASC)
);

GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_tbl_SyncSourceConfig_fld_SourceKey]
    ON [dbo].[tbl_SyncSourceConfig]([fld_SourceKey] ASC);
