CREATE TABLE [dbo].[tbl_ProductStatus] (
    [ProductStatusId]    INT           IDENTITY (1, 1) NOT NULL,
    [ProductStatusTitle] NVARCHAR (50) NULL,
    [ProductStatusCode]  NVARCHAR (50) NULL,
    [ProductStatusDesc]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK__tbl_Prod__2082058B59904A2C] PRIMARY KEY CLUSTERED ([ProductStatusId] ASC)
);

