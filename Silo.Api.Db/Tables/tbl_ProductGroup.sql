CREATE TABLE [dbo].[tbl_ProductGroup] (
    [fld_ProductGroupId]    INT            IDENTITY (1, 1) NOT NULL,
    [fld_ProductGroupCode]  NVARCHAR (128) NOT NULL,
    [fld_ProductGroupTitle] NVARCHAR (128) NOT NULL,
    [fld_ProductGroupData]  NVARCHAR (MAX) NULL
);

