CREATE TABLE [dbo].[tbl_ProductClass] (
    [fld_ProductClassId]    INT IDENTITY (1, 1) NOT NULL,
    [fld_ProductClassCode]  NVARCHAR (128) NOT NULL,
    [fld_ProductClassTitle] NVARCHAR (256) NOT NULL,
    [fld_ProductClassSubTitle]  NVARCHAR (512) NULL,
    [fld_ProductClassDesc]  NVARCHAR (512) NULL
);

