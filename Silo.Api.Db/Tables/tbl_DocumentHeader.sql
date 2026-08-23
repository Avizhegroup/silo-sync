CREATE TABLE [dbo].[tbl_DocumentHeader] (
    [fld_Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [fld_DocumentKey]            NVARCHAR (450) NOT NULL,
    [fld_DocumentSaveUserId]     NVARCHAR (50)  NULL,
    [fld_DocumentImportType]     INT            NULL,
    [fld_DocumentImportFileName] NVARCHAR (512) NULL,
    [fld_DocumentType]           NVARCHAR (10)  NULL,
    [fld_DocumentType1]   NVARCHAR (10)  NULL,
    [fld_DocumentType2]   NVARCHAR (10)  NULL,
    [fld_DocumentImportDatetime] DATETIME       NULL,
    [fld_DocumentDesc]           NVARCHAR (200) NULL,
    [fld_DocumentStatus]         INT            NULL,
    [fld_DocumentHeaderData]     NVARCHAR (MAX) NULL,
    [fld_DocumentParent]         NVARCHAR (512) NULL DEFAULT '0',
    [fld_DocumentAggStatus]      INT, 
    [fld_DocumentDivideParent] NVARCHAR(512) NULL,
    [fld_DocumentChangeStatusLastUserId] NVARCHAR (50)  NULL,
    [fld_DocumentCheckType] int  NULL
    PRIMARY KEY (fld_DocumentKey)
);
