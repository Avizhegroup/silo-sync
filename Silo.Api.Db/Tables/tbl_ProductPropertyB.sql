CREATE TABLE [dbo].[tbl_ProductPropertyB] (
    [fld_ProductPropertyBId]    NVARCHAR (128) NOT NULL,
    [fld_ProductPropertyBTitle] NVARCHAR (512) NOT NULL,
    [fld_ProductPropertyBDesc]  NVARCHAR (MAX) NULL,
    [fld_ProductPropertyBData]  NVARCHAR (MAX) NOT NULL, 
    [fld_ProductPropertyAId] NVARCHAR(128) NULL
);

