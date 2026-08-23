CREATE TABLE [dbo].[tbl_ProductPropertyC] (
    [fld_ProductPropertyCId]       NVARCHAR (128) NOT NULL,
    [fld_ProductPropertyCTitle]    NVARCHAR (256) NOT NULL,
    [fld_ProductPropertyCDesc]     NVARCHAR (MAX) NULL,
    [fld_ProductPropertyCData]     NVARCHAR (MAX) NOT NULL,
    [fld_ProductPropertyCIdentity] INT            IDENTITY (1, 1) NOT NULL
);

