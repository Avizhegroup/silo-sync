CREATE TABLE [dbo].[tbl_CustomerAccountingData] (
    [fld_CADId]   INT  IDENTITY (1, 1) NOT NULL,
    [fld_CADOpCode]     INT NULL,
    [fld_CADDateTime] DATETIME       NULL,
    [fld_CADUser]     NVARCHAR (128) NULL,
    [fld_NDFLName] NVARCHAR (256) NULL,
    [fld_CADProductCode]     NVARCHAR (128) NULL,
    [fld_CADProductCount]     decimal  NULL
);

