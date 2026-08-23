CREATE TABLE [dbo].[tbl_ProductBrand] (
    [fld_ProductBrandId]    INT            IDENTITY (1, 1) NOT NULL,
    [fld_ProductBrandCode]  NVARCHAR (128) NOT NULL,
    [fld_ProductBrandTitle] NVARCHAR (128) NOT NULL,
    [fld_ProductBrandData]  NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([fld_ProductBrandId] ASC)
);

