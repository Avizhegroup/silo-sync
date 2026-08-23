CREATE TABLE [dbo].[tbl_FreezeHeader] (
    [fld_FreezeHeaderId]     INT            IDENTITY (1, 1) NOT NULL,
    [fld_FreezeUserId]       NVARCHAR (128) NULL,
    [fld_FreezeSaveDateTime] DATETIME       NULL,
    [fld_FreezeDesc]         NVARCHAR (256) NULL,
    [fld_FreezeResult]       BIT            NULL
);

