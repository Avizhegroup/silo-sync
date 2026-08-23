CREATE TABLE [dbo].[tbl_ProductUOM](
	[fld_ProductUOMId] [int] IDENTITY(1,1) NOT NULL,
	[fld_ProductCode] [nvarchar](50) NULL,
	[fld_UOMId] [int] NULL,
	[fld_UOMLevel] [int] NULL,
	[fld_ValueInBaseUnit] [decimal](18, 0) NULL,
	[fld_CountInNextlevelUnit] [decimal](18, 0) NULL,
	[fld_ProductUOMParentId] [decimal](18, 0) NULL
) ON [PRIMARY]