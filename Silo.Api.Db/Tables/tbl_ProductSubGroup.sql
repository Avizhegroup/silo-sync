CREATE TABLE [dbo].[tbl_ProductSubGroup] (
    [fld_ProductSubGroupId]    INT            IDENTITY (1, 1) NOT NULL,
    [fld_ProductSubGroupCode]  NVARCHAR (128) NOT NULL,
    [fld_ProductSubGroupTitle] NVARCHAR (256) NOT NULL,
    [fld_ProductSubGroupSubTitle] NVARCHAR (512) NULL,
    [fld_ProductGroupCode]  NVARCHAR (128) NOT NULL,
    [fld_ProductSubGroupDesc] NVARCHAR (512) NULL
);

