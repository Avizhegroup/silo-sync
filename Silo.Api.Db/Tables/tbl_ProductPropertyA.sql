CREATE TABLE [dbo].[tbl_ProductPropertyA] (
    [fld_ProductPropertyAId]    NVARCHAR (128) NOT NULL,
    [fld_ProductPropertyATitle] NVARCHAR (256) NOT NULL,
    [fld_ProductPropertyADesc]  NVARCHAR (MAX) NULL,
    [fld_ProductPropertyAData]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [IX_tbl_ProductPropertyATitleUnique] UNIQUE NONCLUSTERED ([fld_ProductPropertyATitle] ASC)
);

