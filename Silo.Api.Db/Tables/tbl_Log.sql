CREATE TABLE [dbo].[tbl_Log] (
    [fld_Id]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [fld_TableName] NVARCHAR (30)  NOT NULL,
    [fld_UserId]    NVARCHAR (128) NULL,
    [fld_Date]      DATETIME       NOT NULL,
    [fld_OldData]   NVARCHAR (MAX) NOT NULL,
    [fld_TableId]   NVARCHAR (MAX) NOT NULL
);

