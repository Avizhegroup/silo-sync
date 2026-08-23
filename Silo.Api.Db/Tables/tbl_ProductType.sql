CREATE TABLE [dbo].[tbl_ProductType] (
    [ProductTypeId]        INT            IDENTITY (1, 1) NOT NULL,
    [ProductTypeTitle]     NVARCHAR (50)  NULL,
    [ProductTypeParentId]  NVARCHAR (10)  NULL,
    [ProductTypeParentsId] NVARCHAR (MAX) NULL,
    [ProductTypeCode]      NVARCHAR (50)  NULL,
    CONSTRAINT [PK__tbl_Prod__A1312F6E5C6CB6D7] PRIMARY KEY CLUSTERED ([ProductTypeId] ASC)
);

