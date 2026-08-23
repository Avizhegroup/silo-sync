CREATE TABLE [dbo].[tbl_SalesShop] (
    [fld_SalesShopId]       INT  IDENTITY (1, 1) NOT NULL,
    [fld_SalesShopCode]     NVARCHAR (128) NULL,
    [fld_SalesShopTitle]    NVARCHAR (128) NULL,
    [fld_SalesShopManagerName]     NVARCHAR (128) NULL,
    [fld_SalesShopCity]     INT NULL,
    [fld_SalesShopProvince] INT NULL,
    [fld_SalesShopPhone]    NVARCHAR (20) NULL,
    [fld_SalesShopMobile]   NVARCHAR (11) NULL,
    [fld_SalesShopAddress]  NVARCHAR (MAX) NULL,
    [fld_SalesShopUserId]   NVARCHAR (128) NULL
);

