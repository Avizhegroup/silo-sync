CREATE TABLE [dbo].[tbl_DocumentItem] (
    [fld_Id]                       INT             IDENTITY (1, 1) NOT NULL,
    [fld_DocumentKey]              NVARCHAR (512)  NULL,
    [fld_DocumentItemProductCode]  NVARCHAR (50)   NULL,
    [fld_DocumentItemCount]        DECIMAL (18, 2) NULL,
    [fld_DocumentItemProductTitle] NVARCHAR (250)  NULL,
    [fld_DocumentType]             NVARCHAR (10)   NULL,
    [fld_DocumentType1]     NVARCHAR (10)   NULL,
    [fld_DocumentType2]     NVARCHAR (10)   NULL,
    [fld_DocumentItemProducUnit]   NVARCHAR (50)   NULL,
    [fld_DocumentItemsData]        NVARCHAR (MAX)  NULL
);
