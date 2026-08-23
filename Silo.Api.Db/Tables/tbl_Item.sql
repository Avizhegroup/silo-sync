CREATE TABLE [dbo].[tbl_Item] (
    [fld_Id]       INT            IDENTITY (1, 1) NOT NULL,
    [fld_SaveDate] DATETIME       NOT NULL,
    [fld_SaveUser] NVARCHAR (128) NOT NULL,
    [fld_Data]     NVARCHAR (MAX) NULL
);

